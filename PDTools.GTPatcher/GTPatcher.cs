using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

using PDTools.GTPatcher.MemoryPatches;
using PDTools.GTPatcher.BreakLoggers;

namespace PDTools.GTPatcher
{
    public class GTPatcher : IAsyncDisposable
    {
        public PS4DBG PS4 { get; private set; }
        public int GamePid { get; private set; }

        public bool _MemPatchesApplied = false;
        public List<IMemoryPatch> _memoryPatches = new List<IMemoryPatch>();
        public List<IBreakLogger> _breakLoggers = new List<IBreakLogger>();

        public List<Breakpoint> Breakpoints = new List<Breakpoint>();

        public string _ip;

        public readonly ulong ImageBase = 0x400000;

        public const int GTFS_OpenFile = 0x1FF8020;
        public const int LoggerAddress = 0x1C569DD; // rdx = path, r11 = log text

        public GTPatcher(string ip)
        {
            _ip = ip;
        }

        public async Task Start(CancellationToken token)
        {
            PS4 = new PS4DBG(_ip);

            await DebugLoop(token);
        }

        public void AddPatch(IMemoryPatch patch)
        {
            _memoryPatches.Add(patch);
        }

        public void AddBreakLogger(IBreakLogger breakLogger)
        {
            _breakLoggers.Add(breakLogger);
        }

        private async Task DebugLoop(CancellationToken token)
        {
            bool didThing = false;
            while (true)
            {
                if (!PS4.IsConnected)
                {
                    Console.WriteLine($"Attempting to connect to PS4 @ {_ip}...");
                    PS4.Connect();
                    Console.WriteLine("Connected to PS4, now waiting for game process");
                }

                if (!didThing)
                {
                    GamePid = await WaitForProcess("eboot.bin", token);
                    if (token.IsCancellationRequested)
                    {
                        HandleStop();
                        return;
                    }

                    await PS4.AttachDebugger(GamePid, AttachCallback, token);
                    if (token.IsCancellationRequested)
                    {
                        HandleStop();
                        return;
                    }

                    await PS4.Notify(222, "Attached to GTS!");

                    foreach (var breaklogger in _breakLoggers)
                        breaklogger.Init(this);

                    foreach (var patch in _memoryPatches)
                        patch.OnAttach(this);

                    await Task.Delay(1000);
                    await PS4.ProcessResume();

                    await Task.Delay(1000);

                    didThing = true;
                }

                await Task.Delay(1);
            }
        }

        private async Task<int> WaitForProcess(string name, CancellationToken token)
        {
            int pid = 0;
            while (pid == 0)
            {
                if (token.IsCancellationRequested)
                    return -1;

                var list = await PS4.GetProcessList();
                for (var i = 0; i < list.Processes.Length; i++)
                {
                    var process = list.Processes[i];
                    if (process.Name == name)
                    {
                        pid = process.Pid;
                        break;
                    }
                }

                await Task.Delay(50);
            }

            Console.WriteLine($"Found game process: PID {pid}");
            return pid;
        }

        private async void AttachCallback(uint lwpid, uint status, string tdname, GeneralRegisters regs, FloatingPointRegisters fpregs, DebugRegisters dbregs)
        {
            foreach (var breakLogger in _breakLoggers)
            {
                if (breakLogger.CheckHit(this, regs))
                    await breakLogger.OnBreak(this, regs);
            }

            if (!_MemPatchesApplied)
            {
                foreach (var patch in _memoryPatches)
                    await patch.Patch(this, regs);
                
                await PS4.ProcessResume();
                Thread.Sleep(50);
                _MemPatchesApplied = true;

                return;
            }

            await PS4.ProcessResume();
        }

        public async Task<T> ReadMemory<T>(ulong address)
        {
            return await PS4.ReadMemory<T>(GamePid, ImageBase + address);
        }

        public Task<T> ReadMemoryAbsolute<T>(ulong address)
        {
            return PS4.ReadMemory<T>(GamePid, address);
        }

        public async Task WriteMemory<T>(ulong address, T value)
        {
            await PS4.WriteMemory<T>(GamePid, ImageBase + address, value);
        }

        public async Task WriteMemoryAbsolute<T>(ulong address, T value)
        {
            await PS4.WriteMemory<T>(GamePid, address, value);
        }

        public Breakpoint SetBreakpoint(ulong address)
        {
            if (Breakpoints.Count >= 10)
                throw new InvalidOperationException("Too many breakpoints active (no more than 10)");

            var brk = new Breakpoint(Breakpoints.Count, address);
            Breakpoints.Add(brk);

            PS4.ChangeBreakpoint(brk.Index, true, brk.Offset);
            return brk;
        }

        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("Disposing");
            await PS4.DisposeAsync();
        }

        private void HandleStop()
        {
            PS4.Stop();
        }
    }
}
