﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Syroot.BinaryData.Memory;

using PDTools.Enums.PS2;
using PDTools.Utils;
using System.Security.Cryptography;

namespace PDTools.SaveFile.GT4.Option;

public class OptionGT4 : IGameSerializeBase<OptionGT4>
{
    public Locale language { get; set; }
    public bool wide_mode { get; set; }
    
    public UnitVelocityType unit_velocity { get; set; }
    public UnitPowerType unit_power { get; set; }
    public UnitTorqueType unit_torque { get; set; }
    public DTVType race_display_mode { get; set; }
    public int screen_adjust_h { get; set; }
    public int screen_adjust_v { get; set; }
    public int sharpness { get; set; }

    [Browsable(false)]
    public PhysicalMonitorSize monitor_size { get; set; } = new PhysicalMonitorSize();

    public int unk1 { get; set; }
    public int unk2 { get; set; }
    public int unk3 { get; set; }
    public int menu_bgm { get; set; }
    public int menu_bgm_volume { get; set; }
    public int menu_se { get; set; }
    public int menu_se_volume { get; set; }
    public int race_bgm { get; set; }
    public int race_bgm_volume { get; set; }
    public int race_se { get; set; }
    public int race_se_volume { get; set; }
    public int replay_bgm { get; set; }
    public int replay_bgm_volume { get; set; }
    public int replay_se { get; set; }
    public int replay_se_volume { get; set; }
    public int slide_bgm_volume { get; set; }

    [Browsable(false)]
    public BGMPlaylist bgm_playlist { get; set; } = new BGMPlaylist();

    [Browsable(false)]
    public BGMPlaylist slide_playlist { get; set; } = new BGMPlaylist();

    public SoundType sound_type { get; set; }

    public bool narration { get; set; }
    public bool enableAC3 { get; set; }
    public bool auto_save { get; set; }
    public int race_laps { get; set; }
    public int tire_damage { get; set; }
    public AutomaticGear automatic_gear { get; set; }
    public int automatic_ghost { get; set; }
    public int assist_asm { get; set; }
    public int assist_tcs { get; set; }
    public Difficulty difficulty { get; set; }

    public ProfessionalControlType professional_control_1p { get; set; }
    public ProfessionalControlType professional_control_2p { get; set; }

    public PenaltyType penalty_type { get; set; }
    public int limit_favorite_car_only { get; set; }
    public int limit_favorite_course_only { get; set; }
    public int limit_enemies_to_favorite_car_only { get; set; }
    public ViewType view_type { get; set; }
    public ViewType view_type_2p { get; set; }

    public int strict_judgment { get; set; }
    public int display_license_bestline { get; set; }

    public int split_race_laps { get; set; }
    public int split_tire_damage { get; set; }
    public int split_handicap { get; set; }
    public int split_boost_level { get; set; }
    public PhotoQuality photo_quality { get; set; }
    public int photo_manual_focus { get; set; }
    public PhotoAspect photo_aspect { get; set; }
    public ShutterType photo_shutter { get; set; }

    public int photo_professional { get; set; }
    public int photo_saturation { get; set; }
    public int photo_save_slot { get; set; }
    public int photo_save_method { get; set; }
    public int steering_assist_1p{ get; set; }
    public int steering_assist_2p{ get; set; }
    public int active_steering_1p{ get; set; }
    public int active_steering_2p { get; set; }

    public ReplayMode replay_mode { get; set; }
    public int replay_display_enable_flags { get; set; }
    public int replay_2p_split { get; set; }
    public int replay_memory_card_slot { get; set; }
    public int unk { get; set; }
    public int timeout_count_to_autodemo { get; set; }
    public int timeout_count_to_topmenu { get; set; }
    public MachineRole machine_role { get; set; }
    public int special_arcade_tuner { get; set; }
    public int arcade_skip_to_favorite { get; set; }
    public int opening_after_autoload { get; set; }
    public int can_watch_ending { get; set; }
    public int demo_movie_interval { get; set; }
    public int album_slide_effect { get; set; }
    public int album_slide_play_time { get; set; }
    public int album_memory_card_slot { get; set; }
    public int ustorage_photo_quality { get; set; }
    public int album_slide_transition_time { get; set; }

    [Browsable(false)]
    public ColorCorrection race_color_correction { get; set; } = new ColorCorrection();

    [Browsable(false)]
    public ColorCorrection replay_color_correction { get; set; } = new ColorCorrection();

    public string entrance_addr { get; set; }
    public int entrance_port { get; set; }
    public int use_upnp { get; set; }
    public int udp_bind_port_setting { get; set; }
    public int udp_bind_port { get; set; }

    [Browsable(false)]
    public byte[] unk_udpdata { get; set; }

    [Browsable(false)]
    public byte[] unk_end_data { get; set; }

    [Browsable(false)]
    public GameZone GameZone { get; set; } = new GameZone();

    [Browsable(false)]
    public OptionRaceControllerPS2 Controller = new OptionRaceControllerPS2();

    [Browsable(false)]
    public OptionLANBattle LANBattle = new OptionLANBattle();

    [Browsable(false)]
    public OptionNetConfig NetConf = new OptionNetConfig();

    [Browsable(false)]
    public OptionEvent Event = new OptionEvent();

    [Browsable(false)]
    public OptionLogger Logger = new OptionLogger();

    public void CopyTo(OptionGT4 dest)
    {
        dest.language = language;
        dest.wide_mode = wide_mode;

        dest.unit_velocity = unit_velocity;
        dest.unit_power = unit_power;
        dest.unit_torque = unit_torque;
        dest.race_display_mode = race_display_mode;
        dest.screen_adjust_h = screen_adjust_h;
        dest.screen_adjust_v = screen_adjust_v;
        dest.sharpness = sharpness;

        monitor_size.CopyTo(dest.monitor_size);

        dest.unk1 = unk1;
        dest.unk2 = unk2;
        dest.unk3 = unk3;
        dest.menu_bgm = menu_bgm;
        dest.menu_bgm_volume = menu_bgm_volume;
        dest.menu_se = menu_se;
        dest.menu_se_volume = menu_se_volume;
        dest.race_bgm = race_bgm;
        dest.race_bgm_volume = race_bgm_volume;
        dest.race_se = race_se;
        dest.race_se_volume = race_se_volume;
        dest.replay_bgm = replay_bgm;
        dest.replay_bgm_volume = replay_bgm_volume;
        dest.replay_se = replay_se;
        dest.replay_se_volume = replay_se_volume;
        dest.slide_bgm_volume = slide_bgm_volume;

        bgm_playlist.CopyTo(dest.bgm_playlist);
        slide_playlist.CopyTo(dest.slide_playlist);

        dest.sound_type = sound_type;

        dest.narration = narration;
        dest.enableAC3 = enableAC3;
        dest.auto_save = auto_save;
        dest.race_laps = race_laps;
        dest.tire_damage = tire_damage;
        dest.automatic_gear = automatic_gear;
        dest.automatic_ghost = automatic_ghost;
        dest.assist_asm = assist_asm;
        dest.assist_tcs = assist_tcs;
        dest.difficulty = difficulty;

        dest.professional_control_1p = professional_control_1p;
        dest.professional_control_2p = professional_control_2p;

        dest.penalty_type = penalty_type;
        dest.limit_favorite_car_only = limit_favorite_car_only;
        dest.limit_favorite_course_only = limit_favorite_course_only;
        dest.limit_enemies_to_favorite_car_only = limit_enemies_to_favorite_car_only;
        dest.view_type = view_type;
        dest.view_type_2p = view_type_2p;

        dest.strict_judgment = strict_judgment;
        dest.display_license_bestline = display_license_bestline;

        dest.split_race_laps = split_race_laps;
        dest.split_tire_damage = split_tire_damage;
        dest.split_handicap = split_handicap;
        dest.split_boost_level = split_boost_level;
        dest.photo_quality = photo_quality;
        dest.photo_manual_focus = photo_manual_focus;
        dest.photo_aspect = photo_aspect;
        dest.photo_shutter = photo_shutter;

        dest.photo_professional = photo_professional;
        dest.photo_saturation = photo_saturation;
        dest.photo_save_slot = photo_save_slot;
        dest.photo_save_method = photo_save_method;
        dest.steering_assist_1p = steering_assist_1p;
        dest.steering_assist_2p = steering_assist_2p;
        dest.active_steering_1p = active_steering_1p;
        dest.active_steering_2p = active_steering_2p;

        dest.replay_mode = replay_mode;
        dest.replay_display_enable_flags = replay_display_enable_flags;
        dest.replay_2p_split = replay_2p_split;
        dest.replay_memory_card_slot = replay_memory_card_slot;
        dest.unk = unk;
        dest.timeout_count_to_autodemo = timeout_count_to_autodemo;
        dest.timeout_count_to_topmenu = timeout_count_to_topmenu;
        dest.machine_role = machine_role;
        dest.special_arcade_tuner = special_arcade_tuner;
        dest.arcade_skip_to_favorite = arcade_skip_to_favorite;
        dest.opening_after_autoload = opening_after_autoload;
        dest.can_watch_ending = can_watch_ending;
        dest.demo_movie_interval = demo_movie_interval;
        dest.album_slide_effect = album_slide_effect;
        dest.album_slide_play_time = album_slide_play_time;
        dest.album_memory_card_slot = album_memory_card_slot;
        dest.ustorage_photo_quality = ustorage_photo_quality;
        dest.album_slide_transition_time = album_slide_transition_time;
        race_color_correction.CopyTo(dest.race_color_correction);
        replay_color_correction.CopyTo(dest.replay_color_correction);

        dest.entrance_addr = entrance_addr;
        dest.entrance_port = entrance_port;
        dest.use_upnp = use_upnp;
        dest.udp_bind_port_setting = udp_bind_port_setting;
        dest.udp_bind_port = udp_bind_port;

        if (dest.unk_udpdata != null)
        {
            dest.unk_udpdata = new byte[unk_udpdata.Length];
            Array.Copy(unk_udpdata, dest.unk_udpdata, unk_udpdata.Length);
        }

        dest.unk_end_data = new byte[unk_end_data.Length];
        Array.Copy(unk_end_data, dest.unk_end_data, unk_end_data.Length);

        GameZone.CopyTo(dest.GameZone);
        Controller.CopyTo(dest.Controller);
        LANBattle.CopyTo(dest.LANBattle);
        NetConf.CopyTo(dest.NetConf);
        Event.CopyTo(dest.Event);
        Logger.CopyTo(dest.Logger);
    }

    public void Pack(GT4Save save, ref SpanWriter sw)
    {
        if (GT4Save.IsGT4Retail(save.Type))
            sw.WriteInt32(0x11C0);
        else
            sw.WriteInt32(0x1228);

        sw.WriteInt32((int)language);
        sw.WriteByte((byte)unit_velocity);
        sw.WriteByte((byte)unit_power);
        sw.WriteByte((byte)unit_torque);
        sw.WriteBoolean(wide_mode);

        sw.WriteInt32((int)race_display_mode);
        sw.WriteInt32(screen_adjust_h);
        sw.WriteInt32(screen_adjust_v);
        sw.WriteInt32(sharpness);
        monitor_size.Pack(save, ref sw);
        sw.WriteInt32(unk1);
        sw.WriteInt32(unk2);
        sw.WriteInt32(unk3);
        sw.WriteInt32(menu_bgm);
        sw.WriteInt32(menu_bgm_volume);
        sw.WriteInt32(menu_se);
        sw.WriteInt32(menu_se_volume);
        sw.WriteInt32(race_bgm);
        sw.WriteInt32(race_bgm_volume);
        sw.WriteInt32(race_se);
        sw.WriteInt32(race_se_volume);
        sw.WriteInt32(replay_bgm);
        sw.WriteInt32(replay_bgm_volume);
        sw.WriteInt32(replay_se);
        sw.WriteInt32(replay_se_volume);
        sw.WriteInt32(slide_bgm_volume);

        bgm_playlist.Pack(save, ref sw);
        slide_playlist.Pack(save, ref sw);

        sw.WriteInt32((int)sound_type);
        sw.WriteBoolean4(narration);
        sw.WriteBoolean4(enableAC3);
        sw.WriteBoolean4(auto_save);
        sw.WriteInt32(race_laps);
        sw.WriteInt32(tire_damage);
        sw.WriteInt32((int)automatic_gear);
        sw.WriteInt32(automatic_ghost);
        sw.WriteInt32(assist_asm);
        sw.WriteInt32(assist_tcs);
        sw.WriteInt32((int)difficulty);
        sw.WriteInt32((int)professional_control_1p);
        sw.WriteInt32((int)professional_control_2p);
        sw.WriteInt32((int)penalty_type);
        sw.WriteInt32(limit_favorite_car_only);
        sw.WriteInt32(limit_favorite_course_only);
        sw.WriteInt32(limit_enemies_to_favorite_car_only);
        sw.WriteInt32((int)view_type);
        sw.WriteInt32((int)view_type_2p);

        if (GT4Save.IsGT4Online(save.Type))
        {
            sw.WriteInt32(strict_judgment);
            sw.WriteInt32(display_license_bestline);
        }

        sw.WriteInt32(split_race_laps);
        sw.WriteInt32(split_tire_damage);
        sw.WriteInt32(split_handicap);
        sw.WriteInt32(split_boost_level);
        sw.WriteInt32((int)photo_quality);
        sw.WriteInt32(photo_manual_focus);
        sw.WriteInt32((int)photo_aspect);
        sw.WriteInt32((int)photo_shutter);
        sw.WriteInt32(photo_professional);
        sw.WriteInt32(photo_saturation);
        sw.WriteInt32(photo_save_slot);
        sw.WriteInt32(photo_save_method);
        sw.WriteInt32(steering_assist_1p);
        sw.WriteInt32(steering_assist_2p);
        sw.WriteInt32(active_steering_1p);
        sw.WriteInt32(active_steering_2p);

        sw.WriteInt32((int)replay_mode);
        sw.WriteInt32(replay_display_enable_flags);
        sw.WriteInt32(replay_2p_split);
        sw.WriteInt32(replay_memory_card_slot);
        sw.WriteInt32(unk);
        sw.WriteInt32(timeout_count_to_autodemo);
        sw.WriteInt32(timeout_count_to_topmenu);
        sw.WriteInt32((int)machine_role);
        sw.WriteInt32(special_arcade_tuner);
        sw.WriteInt32(arcade_skip_to_favorite);
        sw.WriteInt32(opening_after_autoload);
        sw.WriteInt32(can_watch_ending);
        sw.WriteInt32(demo_movie_interval);
        sw.WriteInt32(album_slide_effect);
        sw.WriteInt32(album_slide_play_time);
        sw.WriteInt32(album_memory_card_slot);
        sw.WriteInt32(ustorage_photo_quality);
        sw.WriteInt32(album_slide_transition_time);

        race_color_correction.Pack(save, ref sw);
        replay_color_correction.Pack(save, ref sw);

        if (GT4Save.IsGT4Online(save.Type))
        {
            sw.WriteStringFix(entrance_addr, 64);
            sw.WriteInt32(entrance_port);
            sw.WriteInt32(use_upnp);
            sw.WriteInt32(udp_bind_port_setting);
            sw.WriteInt32(udp_bind_port);
            sw.WriteBytes(unk_udpdata);
        }

        sw.WriteBytes(unk_end_data);

        sw.Align(GT4Save.ALIGNMENT);

        GameZone.Pack(save, ref sw);
        Controller.Pack(save, ref sw);
        LANBattle.Pack(save, ref sw);
        NetConf.Pack(save, ref sw);
        Event.Pack(save, ref sw);
        Logger.Pack(save, ref sw);
    }

    public void Unpack(GT4Save save, ref SpanReader sr)
    {
        int optionSize = sr.ReadInt32();
        language = (Locale)sr.ReadInt32();
        unit_velocity = (UnitVelocityType)sr.ReadByte();
        unit_power = (UnitPowerType)sr.ReadByte();
        unit_torque = (UnitTorqueType)sr.ReadByte();
        wide_mode = sr.ReadBoolean();

        race_display_mode = (DTVType)sr.ReadInt32();
        screen_adjust_h = sr.ReadInt32();
        screen_adjust_v = sr.ReadInt32();
        sharpness = sr.ReadInt32();
        monitor_size.Unpack(save, ref sr);
        unk1 = sr.ReadInt32();
        unk2 = sr.ReadInt32();
        unk3 = sr.ReadInt32();
        menu_bgm = sr.ReadInt32();
        menu_bgm_volume = sr.ReadInt32();
        menu_se = sr.ReadInt32();
        menu_se_volume = sr.ReadInt32();
        race_bgm = sr.ReadInt32();
        race_bgm_volume = sr.ReadInt32();
        race_se = sr.ReadInt32();
        race_se_volume = sr.ReadInt32();
        replay_bgm = sr.ReadInt32();
        replay_bgm_volume = sr.ReadInt32();
        replay_se = sr.ReadInt32();
        replay_se_volume = sr.ReadInt32();
        slide_bgm_volume = sr.ReadInt32();

        bgm_playlist.Unpack(save, ref sr);
        slide_playlist.Unpack(save, ref sr);

        sound_type = (SoundType)sr.ReadInt32();
        narration = sr.ReadBoolean4();
        enableAC3 = sr.ReadBoolean4();
        auto_save = sr.ReadBoolean4();
        race_laps = sr.ReadInt32();
        tire_damage = sr.ReadInt32();
        automatic_gear = (AutomaticGear)sr.ReadInt32();
        automatic_ghost = sr.ReadInt32();
        assist_asm = sr.ReadInt32();
        assist_tcs = sr.ReadInt32();
        difficulty = (Difficulty)sr.ReadInt32();
        professional_control_1p = (ProfessionalControlType)sr.ReadInt32();
        professional_control_2p = (ProfessionalControlType)sr.ReadInt32();
        penalty_type = (PenaltyType)sr.ReadInt32();
        limit_favorite_car_only = sr.ReadInt32();
        limit_favorite_course_only = sr.ReadInt32();
        limit_enemies_to_favorite_car_only = sr.ReadInt32();
        view_type = (ViewType)sr.ReadInt32();
        view_type_2p = (ViewType)sr.ReadInt32();

        if (GT4Save.IsGT4Online(save.Type))
        {
            strict_judgment = sr.ReadInt32();
            display_license_bestline = sr.ReadInt32();
        }

        split_race_laps = sr.ReadInt32();
        split_tire_damage = sr.ReadInt32();
        split_handicap = sr.ReadInt32();
        split_boost_level = sr.ReadInt32();
        photo_quality = (PhotoQuality)sr.ReadInt32();
        photo_manual_focus = sr.ReadInt32();
        photo_aspect = (PhotoAspect)sr.ReadInt32();
        photo_shutter = (ShutterType)sr.ReadInt32();
        photo_professional = sr.ReadInt32();
        photo_saturation = sr.ReadInt32();
        photo_save_slot = sr.ReadInt32();
        photo_save_method = sr.ReadInt32();
        steering_assist_1p = sr.ReadInt32();
        steering_assist_2p = sr.ReadInt32();
        active_steering_1p = sr.ReadInt32();
        active_steering_2p = sr.ReadInt32();

        replay_mode = (ReplayMode)sr.ReadInt32();
        replay_display_enable_flags = sr.ReadInt32();
        replay_2p_split = sr.ReadInt32();
        replay_memory_card_slot = sr.ReadInt32();
        unk = sr.ReadInt32();
        timeout_count_to_autodemo = sr.ReadInt32();
        timeout_count_to_topmenu = sr.ReadInt32();
        machine_role = (MachineRole)sr.ReadInt32();
        special_arcade_tuner = sr.ReadInt32();
        arcade_skip_to_favorite = sr.ReadInt32();
        opening_after_autoload = sr.ReadInt32();
        can_watch_ending = sr.ReadInt32();
        demo_movie_interval = sr.ReadInt32();
        album_slide_effect = sr.ReadInt32();
        album_slide_play_time = sr.ReadInt32();
        album_memory_card_slot = sr.ReadInt32();
        ustorage_photo_quality = sr.ReadInt32();
        album_slide_transition_time = sr.ReadInt32();

        race_color_correction.Unpack(save, ref sr);
        replay_color_correction.Unpack(save, ref sr);

        if (GT4Save.IsGT4Online(save.Type))
        {
            entrance_addr = sr.ReadFixedString(64);
            entrance_port = sr.ReadInt32();
            use_upnp = sr.ReadInt32();
            udp_bind_port_setting = sr.ReadInt32();
            udp_bind_port = sr.ReadInt32();
            unk_udpdata = sr.ReadBytes(0x10);
        }

        unk_end_data = sr.ReadBytes(0x24);

        sr.Align(GT4Save.ALIGNMENT);

        GameZone.Unpack(save, ref sr);
        Controller.Unpack(save, ref sr);
        LANBattle.Unpack(save, ref sr);
        NetConf.Unpack(save, ref sr);
        Event.Unpack(save, ref sr);
        Logger.Unpack(save, ref sr);
    }
}
