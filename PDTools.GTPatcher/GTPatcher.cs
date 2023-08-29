using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libdebug;

using PDTools.GTPatcher.MemoryPatches;
using PDTools.GTPatcher.BreakLoggers;
using System.Security.AccessControl;

namespace PDTools.GTPatcher
{
    public class GTPatcher : IDisposable
    {
        public PS4DBG PS4 { get; private set; }
        public int GamePid { get; private set; }

        public GameType GameType { get; }
        public bool _MemPatchesApplied = false;
        public List<IMemoryPatch> _memoryPatches = new List<IMemoryPatch>();
        public List<IBreakLogger> _breakLoggers = new List<IBreakLogger>();

        public List<Breakpoint> Breakpoints = new List<Breakpoint>();

        public string _ip;

        public readonly ulong ImageBase = 0x400000;

        public const int GTFS_OpenFile = 0x1FF8020;
        public const int LoggerAddress = 0x1C569DD; // rdx = path, r11 = log text

        public GTPatcher(string ip, GameType gameType)
        {
            _ip = ip;
            GameType = gameType;
        }

        public void Start(CancellationToken token)
        {
            PS4 = new PS4DBG(_ip);

            foreach (var i in _memoryPatches)
                i.Init(this);

            DebugLoop(token);
        }

        ~GTPatcher()
        {
            Dispose();
        }

        public void AddPatch(IMemoryPatch patch)
        {
            _memoryPatches.Add(patch);
        }

        public void AddBreakLogger(IBreakLogger breakLogger)
        {
            _breakLoggers.Add(breakLogger);
        }

        private void DebugLoop(CancellationToken token)
        {
            bool didThing = false;
            while (!token.IsCancellationRequested)
            {
                if (!PS4.IsConnected)
                {
                    Console.WriteLine($"Attempting to connect to PS4 @ {_ip}...");
                    PS4.Connect();
                    Console.WriteLine("Connected to PS4, now waiting for game process");
                }

                if (!didThing)
                {
                    GamePid = WaitForProcess("eboot.bin");
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    PS4.AttachDebugger(GamePid, AttachCallback);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    PS4.Notify(222, $"Attached to: {GameType}");

                    foreach (var breaklogger in _breakLoggers)
                        breaklogger.Init(this);

                    foreach (var patch in _memoryPatches)
                        patch.OnAttach(this);

                    Thread.Sleep(1000);
                    PS4.ProcessResume();

                    Thread.Sleep(1000);

                    didThing = true;
                }

                Thread.Sleep(100);
            }
        }


        private int WaitForProcess(string name)
        {
            int pid = 0;
            while (pid == 0)
            {
                var list = PS4.GetProcessList();
                for (var i = 0; i < list.Processes.Length; i++)
                {
                    var process = list.Processes[i];
                    if (process.Name == name)
                    {
                        pid = process.Pid;
                        break;
                    }
                }

                Thread.Sleep(50);
            }

            Console.WriteLine($"Found game process: PID {pid}");
            return pid;
        }

        private void AttachCallback(uint lwpid, uint status, string tdname, GeneralRegisters regs, FloatingPointRegisters fpregs, DebugRegisters dbregs)
        {
            for (var i = _breakLoggers.Count - 1; i >= 0; i--)
            {
                var log = _breakLoggers[i];
                if (log.CheckHit(this, regs))
                    log.OnBreak(this, regs);
            }

            if (!_MemPatchesApplied)
            {
                foreach (var patch in _memoryPatches)
                    patch.Patch(this, regs);
                
                PS4.ProcessResume();
                Thread.Sleep(50);
                _MemPatchesApplied = true;

                return;
            }

            PS4.ProcessResume();
        }

        // Don't mind, game on debug sends requests to dev url server
        // Hijacked it, turns out it's just analytics "/analytics/report_error_code" which connection fails..
        // While at it, nothing that important in the request headers
        // Agent is: CppVC/4.73.1 (PS4) - C++ Vegas Client?
        // Accept-Encoding: gzip, deflate
        // body: { "code": "CE-210717" }

        /*
        await dbg.WriteMemory<string>(0x3C81D8A, "http://{service}.{stage}.{base_domain}/auth"); // Activity
        await dbg.WriteMemory<string>(0x3C81E03, "http://{service}.{stage}.{base_domain}/analytics"); // Analytics
        await dbg.WriteMemory<string>(0x3C81F7F, "http://{service}.{stage}.{base_domain}/auth"); // Auth
        await dbg.WriteMemory<string>(0x3C839A6, "http://{service}.{stage}.{base_domain}/save"); // Save
        await dbg.WriteMemory<string>(0x3C8405F, "http://{service}.{stage}.{base_domain}/user"); // User
        */

        /*
        await dbg.WriteMemory<string>(0x3C81D8A, "http://<pub ip>/auth"); // Activity
        await dbg.WriteMemory<string>(0x3C81E03, "http://<pub ip>/analytics"); // Analytics
        await dbg.WriteMemory<string>(0x3C81F7F, "http://<pub ip>/auth"); // Auth
        await dbg.WriteMemory<string>(0x3C839A6, "http://<pub ip>/save"); // Save
        await dbg.WriteMemory<string>(0x3C8405F, "http://<pub ip>/user"); // User
        */

        public void ReadMemory(ulong address, byte[] buffer, int length)
        {
            PS4.ReadMemory(buffer, GamePid, ImageBase + address, length);
        }

        public T ReadMemory<T>(ulong address)
        {
            return PS4.ReadMemory<T>(GamePid, ImageBase + address);
        }

        public T ReadMemoryAbsolute<T>(ulong address)
        {
            return PS4.ReadMemory<T>(GamePid, address);
        }

        public void WriteMemory<T>(ulong address, T value)
        {
            PS4.WriteMemory<T>(GamePid, ImageBase + address, value);
        }

        public void WriteMemoryAbsolute<T>(ulong address, T value)
        {
            PS4.WriteMemory<T>(GamePid, address, value);
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

        public void UpdateBreakpoint(Breakpoint brk, ulong absoluteOffset)
        {
            brk.Offset = absoluteOffset;
            PS4.ChangeBreakpoint(brk.Index, true, brk.Offset);
        }

        public void Notify(string message)
        {
            PS4.Notify(222, message);
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing");
            PS4.Dispose();
        }
    }

    public enum GameType
    {
        GTS_V168,
        GT7_V100,
        GT7_V125,
        GT7_V129,
        GT7_V136,
    }
}
