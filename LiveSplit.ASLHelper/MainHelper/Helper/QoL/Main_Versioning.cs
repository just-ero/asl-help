using LiveSplit.ComponentUtil;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ASLHelper
{
	public partial class Main
	{
		public int GetMemorySize()
        {
			var module = Game?.MainModuleWow64Safe();
			return GetMemorySize(module);
		}

		public int GetMemorySize(string moduleName)
		{
			var module = GetModule(moduleName);
			return GetMemorySize(module);
		}

		public int GetMemorySize(ProcessModuleWow64Safe module)
		{
			if (module == null)
			{
				Debug.Warn("[GetHash] Module could not be found!");
				return 0;
			}

			return module.ModuleMemorySize;
		}

		public string GetMD5Hash()
		{
			var module = Game?.MainModuleWow64Safe();
			return GetMD5Hash(module);
		}

		public string GetMD5Hash(string moduleName)
		{
			var module = GetModule(moduleName);
			return GetMD5Hash(module);
		}

		public string GetMD5Hash(ProcessModuleWow64Safe module)
		{
			using (var md5 = MD5.Create())
				return GetHash(module, md5);
		}

		public string GetSHA1Hash()
		{
			var module = Game?.MainModuleWow64Safe();
			return GetSHA1Hash(module);
		}

		public string GetSHA1Hash(string moduleName)
		{
			var module = GetModule(moduleName);
			return GetSHA1Hash(module);
		}

		public string GetSHA1Hash(ProcessModuleWow64Safe module)
		{
			using (var sha1 = SHA1.Create())
				return GetHash(module, sha1);
		}

		public string GetSHA256Hash()
		{
			var module = Game?.MainModuleWow64Safe();
			return GetSHA256Hash(module);
		}

		public string GetSHA256Hash(string moduleName)
		{
			var module = GetModule(moduleName);
			return GetSHA256Hash(module);
		}

		public string GetSHA256Hash(ProcessModuleWow64Safe module)
		{
			using (var sha256 = SHA256.Create())
				return GetHash(module, sha256);
		}

		public string GetSHA384Hash()
		{
			var module = Game?.MainModuleWow64Safe();
			return GetSHA384Hash(module);
		}

		public string GetSHA384Hash(string moduleName)
		{
			var module = GetModule(moduleName);
			return GetSHA384Hash(module);
		}

		public string GetSHA384Hash(ProcessModuleWow64Safe module)
		{
			using (var sha384 = SHA384.Create())
				return GetHash(module, sha384);
		}

		public string GetSHA512Hash()
		{
			var module = Game?.MainModuleWow64Safe();
			return GetSHA512Hash(module);
		}

		public string GetSHA512Hash(string moduleName)
		{
			var module = GetModule(moduleName);
			return GetSHA512Hash(module);
		}

		public string GetSHA512Hash(ProcessModuleWow64Safe module)
		{
			using (var sha512 = SHA512.Create())
				return GetHash(module, sha512);
		}

		private string GetHash(ProcessModuleWow64Safe module, HashAlgorithm algorithm)
		{
			if (module == null)
			{
				Debug.Warn("[GetHash] Module could not be found!");
				return null;
			}

			using (var reader = File.OpenRead(module.FileName))
				return string.Concat(algorithm.ComputeHash(reader).Select(b => $"{b:X2}"));
		}
	}
}