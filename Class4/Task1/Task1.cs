namespace Task1
{
    // Необходимо заменить на более подходящий тип (коллекцию), позволяющий
    // эффективно искать диапазон по заданному IP-адресу
    using IPRangesDatabase = List<Task1.IPRange>;

    public class Task1
    {
        /*
        * Объекты этого класса создаются из строки, но хранят внутри помимо строки
        * ещё и целочисленное значение соответствующего адреса. Например, для адреса
         * 127.0.0.1 должно храниться число 1 + 0 * 2^8 + 0 * 2^16 + 127 * 2^24 = 2130706433.
        */
        internal record IPv4Addr(string StrValue) : IComparable<IPv4Addr>
        {
            internal uint IntValue = Ipstr2Int(StrValue);

            private static uint Ipstr2Int(string strValue)
            {
                var parts = strValue.Split('.').Select(byte.Parse).Reverse().ToArray();
                if (parts.Length != 4)
                    throw new Exception($"Ip {strValue} incorrect");
                return BitConverter.ToUInt32(parts, 0);
            }

            // Благодаря этому методу мы можем сравнивать два значения IPv4Addr
            public int CompareTo(IPv4Addr? other)
            {
                if (other == null)
                    return -1;
                return IntValue.CompareTo(other.IntValue);
            }

            public override string ToString()
            {
                return StrValue;
            }
        }

        internal record IPRange(IPv4Addr IpFrom, IPv4Addr IpTo)
        {
            public static IPRange? Parse(string line)
            {
                return line.Split(',') is [var ipFrom, var ipTo]
                    ? new IPRange(new IPv4Addr(ipFrom), new IPv4Addr(ipTo))
                    : null;
            }

            public bool Contains(IPv4Addr ip)
            {
                // IpFrom <= ip <= IpTo
                return IpFrom.CompareTo(ip) <= 0 && ip.CompareTo(IpTo) <= 0;
            }

            public override string ToString()
            {
                return $"{IpFrom},{IpTo}";
            }
        }

        internal record IPLookupArgs(string IpsFile, List<string> IprsFiles)
        {
            public virtual bool Equals(IPLookupArgs? other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return IpsFile == other.IpsFile && IprsFiles.SequenceEqual(other.IprsFiles);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(IpsFile, IprsFiles);
            }
        }

        internal static IPLookupArgs? ParseArgs(string[] args)
        {
            if (args.Length < 2)
                return null;
            var ipsFile = args[0];
            var iprsFiles = args[1..].ToList();
            return new IPLookupArgs(ipsFile, iprsFiles);
        }

        internal static List<string> LoadQuery(string filename)
        {
            return File.ReadLines(filename).ToList();
        }


        internal static IPRangesDatabase LoadRanges(List<String> filenames)
        {
            var db = new List<IPRange>();
            foreach (var filename in filenames)
                db.AddRange(File.ReadLines(filename).Select(IPRange.Parse).Where(range => range != null)!);

            return db;
        }

        internal static IPRange? FindRange(IPRangesDatabase ranges, IPv4Addr query)
        {
            return ranges.Find(range => range.Contains(query));
        }

        public static void Main(string[] args)
        {
            var ipLookupArgs = ParseArgs(args);
            if (ipLookupArgs == null)
                return;

            var outName = Path.ChangeExtension(ipLookupArgs.IpsFile, ".ips.out");
            var writer = new StreamWriter(outName);
            var queries = LoadQuery(ipLookupArgs.IpsFile);
            var ranges = LoadRanges(ipLookupArgs.IprsFiles);
            foreach (var ip in queries)
            {
                var range = FindRange(ranges, new IPv4Addr(ip));
                var result = range == null ? "NO" : $"YES {range}";
                writer.WriteLine($"{ip}: {result}");
            }

            writer.Close();
        }
    }
}