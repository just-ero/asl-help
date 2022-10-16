using AslHelp.MemUtils.SigScan;

public partial class Unreal
{
    /// <summary>
    ///     The <see cref="Unreal.Data"/> class exists to pre-compile any <see cref="Signature"/>s
    ///     used later in the code.
    /// </summary>
    private static class Data
    {
        private static readonly Unreal _game;

        static Data()
        {
            _game = Instance;
        }

        public static Signature GEngineTrg_64
            => new(15, "40 53 48 83 EC 20 48 8B D9 48 8B D1 48 8D 0D ???????? 41 B8 04 00 00 00 E8 ???????? 48 8B C3 48 83 C4 20 5B C3") { Name = "GEngineVersion" };

        public static Signature FEngineTrg_64
            => new(10, "40 53 48 83 EC 20 48 8B D9 E8 ???????? 48 8B C8 41 B8 04 00 00 00 48 8B D3 E8 ???????? 48 8B C3 48 83 C4 20 5B C3") { Name = "FEngineVersion::Current" };

        public static Signature FEngineTrg_CurrAbs_64
            => new(3, "48 8D 05 ???????? C3");

        public static Signature FEngineTrg_CurrRel_64
            => new(10, "75 ?? 48 89 5C 24 ?? 48 8D 1D");

        public static Signature GEngineTrg_32
            => new(7, "6A 04 FF 74 24 ?? B9 ???????? E8 ???????? 8B 44 24 ?? C3") { Name = "GEngineVersion" };

        public static Signature FEngineTrg_32
            => new(7, "6A 04 FF 74 24 ?? E8 ???????? 8B C8 E8 ???????? 8B 44 24 ?? C3") { Name = "FEngineVersion::Current" };

        public static Signature FEngineTrg_CurrAbs_32
            => new(1, "B8 ???????? C3");

        public static Signature FEngineTrg_CurrRel_32
            => new(3, "75 ?? 68 ???????? E8");

        public static nint EngineTrg_OnFound_64(string name, nint address)
        {
            address = _game.FromRelativeAddress(address);

            if (name == "GEngineVersion")
            {
                return address;
            }

            nint fEngineVersionCurrent = _game.ScanRel(FEngineTrg_CurrAbs_64, address, 0x9);

            if (fEngineVersionCurrent != 0)
            {
                return fEngineVersionCurrent;
            }

            return _game.ScanRel(FEngineTrg_CurrRel_64, _game.FromRelativeAddress(address + 0x1), 0x80);
        }

        public static nint EngineTrg_OnFound_32(string name, nint address)
        {
            address = _game.FromRelativeAddress(address);

            if (name == "GEngineVersion")
            {
                return address;
            }

            nint fEngineVersionCurrent = _game.ScanRel(FEngineTrg_CurrAbs_32, address, 0x7);

            if (fEngineVersionCurrent != 0)
            {
                return fEngineVersionCurrent;
            }

            return _game.ScanRel(FEngineTrg_CurrRel_32, _game.FromRelativeAddress(address + 0x1), 0x80);
        }
    }
}
