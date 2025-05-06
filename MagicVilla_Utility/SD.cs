using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_Utility;
public static class SD
{

	public enum ApiType
	{
		GET,
		POST,
		PUT,
		DELETE
	}

	public readonly static string SessionKey = "JwtToken";
	public readonly static string Version = "v1";

}
