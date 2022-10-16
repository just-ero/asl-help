namespace AslHelp.MemUtils.SigScan;

public record Signature
{
    public Signature(int offset, params string[] pattern)
    {
        if (pattern is null)
        {
            string msg = "Signature pattern was null.";
            throw new ArgumentNullException(msg);
        }

        if (pattern.Length == 0)
        {
            string msg = "Signature pattern was empty.";
            throw new ArgumentException(msg);
        }

        string signature = pattern.Concat().RemoveWhiteSpace();

        if (signature.Length % 2 != 0)
        {
            string msg = "Signature was in an incorrect format. All bytes must be fully specified.";
            throw new FormatException(msg);
        }

        Offset = offset;
        Bytes = ParseBytes(signature);
    }

    public Signature(params string[] pattern)
        : this(0, pattern) { }

    public Signature(int offset, params byte[] pattern)
    {
        if (pattern is null)
        {
            string msg = "Signature pattern was null.";
            throw new ArgumentNullException(msg);
        }

        if (pattern.Length == 0)
        {
            string msg = "Signature pattern was empty.";
            throw new ArgumentException(msg);
        }

        Offset = offset;
        Bytes = ParseBytes(pattern);
    }

    public Signature(params byte[] pattern)
        : this(0, pattern) { }

    public string Name { get; set; }
    public int Offset { get; }
    public (int Length, byte[] Values, byte[] Masks) Bytes { get; }

    private static unsafe (int, byte[], byte[]) ParseBytes(string signature)
    {
        int length = signature.Length / 2;
        byte[] values = new byte[length], masks = new byte[length];

        fixed (char* pSignature = signature)
        fixed (byte* pValues = values, pMasks = masks)
        {
            for (int i = 0, j = 0; i < signature.Length; i += 2, j++)
            {
                char cUpper = pSignature[i + 0], cLower = pSignature[i + 1];

                bool hasUpper = cUpper.IsHexDigit(out byte upper);
                bool hasLower = cLower.IsHexDigit(out byte lower);

                (pValues[j], pMasks[j]) = (hasUpper, hasLower) switch
                {
                    (true, true) => ((byte)((upper << 4) + lower), (byte)0xFF),
                    (true, false) => ((byte)(upper << 4), (byte)0xF0),
                    (false, true) => (lower, (byte)0x0F),
                    (false, false) => ((byte)0x00, (byte)0x00)
                };
            }
        }

        return (length, values, masks);
    }

    private static unsafe (int, byte[], byte[]) ParseBytes(byte[] signature)
    {
        int length = signature.Length;
        byte[] values = new byte[length], masks = new byte[length];

        fixed (byte* pSignature = signature, pValues = values, pMasks = masks)
        {
            for (int i = 0; i < length; i++)
            {
                pValues[i] = pSignature[i];
                pMasks[i] = 0xFF;
            }
        }

        return (length, values, masks);
    }
}
