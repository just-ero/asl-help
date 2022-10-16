using AslHelp.MemUtils.SigScan;

namespace AslHelp.UE.Managers;

internal partial class UE4_0Manager
{
    protected override ScanTarget _fNamesTrg
    {
        get
        {
            ScanTarget trg = new()
            {
                { 1, "A1 ???????? 85 C0 75 ?? 56 68 08 02 00 00" }
            };

            trg.OnFound = (_, addr) => _game.FromAssemblyAddress(addr);

            return trg;
        }
    }

    protected override ScanTarget _uObjectsTrg
    {
        get
        {
            ScanTarget trg = new()
            {
                { 1, "A3 ???????? 85 C0 74 ?? 3B 05" }
            };

            trg.OnFound = (_, addr) => _game.FromAssemblyAddress(addr);

            return trg;
        }
    }
}

internal partial class UE4_8Manager
{
    protected override ScanTarget _fNamesTrg
    {
        get
        {
            ScanTarget trg = new(); // base._fNamesTrg

            if (_game.Is64Bit)
            {
                trg.Add(7, "48 83 EC 28 48 8B 05 ???????? 48 85 C0 75 ?? B9 08 0? 00 00");
            }
            else
            {
                trg.Add(1, "A1 ???????? 85 C0 75 ?? 56 68 08 02 00 00");
            }

            trg.OnFound = (_, addr) => _game.FromAssemblyAddress(addr);

            return trg;
        }
    }

    protected override ScanTarget _uObjectsTrg
    {
        get
        {
            ScanTarget trg = new(); // base._uObjectsTrg

            if (_game.Is64Bit)
            {
                trg.Add(10, "48 89 43 ?? E8 ???????? E8 ???????? 48 8B ?? 48 8B ?? 48 83 C4 20");

                trg.OnFound = (_, addr) =>
                {
                    addr = _game.FromAssemblyAddress(addr);
                    return _game.ScanRel(addr, 0x100, 3, "48 8D 05 ???????? 48 83 C4 20 5B C3");
                };
            }
            else
            {
                trg.Add(5, "83 C4 08 56 E8 ???????? 8B C8 E8 ???????? 5E");

                trg.OnFound = (_, addr) =>
                {
                    addr = _game.FromAssemblyAddress(addr);
                    return _game.ScanRel(addr, 0x100, 4, "83 C4 ?? B8 ???????? C3");
                };
            }

            return trg;
        }
    }
}

internal partial class UE4_11Manager
{
    protected override ScanTarget _uObjectsTrg
    {
        get
        {
            ScanTarget trg = new(); // base._uObjectsTrg

            if (_game.Is64Bit)
            {
                trg.Add(18, "48 8B CB 48 89 43 ?? E8 ???????? 48 8B D3 48 8D 0D ???????? 48 83 C4 ?0");

                trg.OnFound = (_, addr) => _game.FromAssemblyAddress(addr);
            }
            else
            {
                trg.Add(1, "B9 ???????? 56 E8 ???????? 5E C3");

                trg.OnFound = (_, addr) => _game.FromAssemblyAddress(addr);
            }

            return trg;
        }
    }
}

internal partial class UE4_20Manager
{
    protected override ScanTarget _uObjectsTrg
    {
        get
        {
            ScanTarget trg = new(); // base._uObjectsTrg

            if (_game.Is64Bit)
            {
                trg.Add(9, "44 0F B6 4C 24 ?? 48 8D 0D ???????? 44 8B ?4 24");

                trg.OnFound = (_, addr) => _game.FromAssemblyAddress(addr);
            }
            else
            {
                trg.Add(1, "B9 ???????? 50 89 06");

                trg.OnFound = (_, addr) => _game.FromAssemblyAddress(addr);
            }

            return trg;
        }
    }
}

internal partial class UE4_23Manager
{
    protected override ScanTarget _fNamesTrg
    {
        get
        {
            ScanTarget trg = new(); // base._fNamesTrg

            if (_game.Is64Bit)
            {
                trg.Add(13, "89 5C 24 ?? 89 44 24 ?? 74 ?? 48 8D 15");

                trg.OnFound = (_, addr) => _game.FromAssemblyAddress(addr);
            }
            else
            {
                trg.Add(7, "57 0F B7 F8 74 ?? B8 ???????? 8B 44");

                trg.OnFound = (_, addr) => _game.FromAssemblyAddress(addr);
            }

            return trg;
        }
    }

    protected override ScanTarget _uObjectsTrg
    {
        get
        {
            ScanTarget trg = new(); // base._uObjectsTrg

            if (_game.Is64Bit)
            {
                trg.Add(3, "48 8D 0D ???????? 44 8B 84 24");

                trg.OnFound = (_, addr) => _game.FromAssemblyAddress(addr);
            }
            else
            {
                trg.Add(2, "89 0D ???????? 85 F6 7F");

                trg.OnFound = (_, addr) => _game.FromAssemblyAddress(addr);
            }

            return trg;
        }
    }
}

internal partial class UE4_25Manager
{
    protected override ScanTarget _uObjectsTrg
    {
        get
        {
            ScanTarget trg = base._uObjectsTrg;

            trg.Add(2, "89 15 ???????? 85 FF");

            return trg;
        }
    }
}
