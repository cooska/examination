using System;
using System.Security.Cryptography;  
using System.Text;
namespace Maticsoft.Common.DEncrypt
{
	/// <summary>
	/// DES加密/解密类。
    /// Copyright (C) Maticsoft
	/// </summary>
	public class DESEncrypt
	{
		public DESEncrypt()
		{			
		}

		#region ========加密======== 
 
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
		public static string Encrypt(string Text) 
		{
			return Encrypt(Text,"coos");
		}
		/// <summary> 
		/// 加密数据 
		/// </summary> 
		/// <param name="Text"></param> 
		/// <param name="sKey"></param> 
		/// <returns></returns> 
		public static string Encrypt(string Text,string sKey) 
		{ 
			DESCryptoServiceProvider des = new DESCryptoServiceProvider(); 
			byte[] inputByteArray; 
			inputByteArray=Encoding.Default.GetBytes(Text); 
			des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8)); 
			des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8)); 
			System.IO.MemoryStream ms=new System.IO.MemoryStream(); 
			CryptoStream cs=new CryptoStream(ms,des.CreateEncryptor(),CryptoStreamMode.Write); 
			cs.Write(inputByteArray,0,inputByteArray.Length); 
			cs.FlushFinalBlock(); 
			StringBuilder ret=new StringBuilder(); 
			foreach( byte b in ms.ToArray()) 
			{ 
				ret.AppendFormat("{0:X2}",b); 
			} 
			return ret.ToString(); 
		} 

		#endregion
		
		#region ========解密======== 
   
 
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
		public static string Decrypt(string Text) 
		{
			return Decrypt(Text,"coos");
		}
		/// <summary> 
		/// 解密数据 
		/// </summary> 
		/// <param name="Text"></param> 
		/// <param name="sKey"></param> 
		/// <returns></returns> 
		public static string Decrypt(string Text,string sKey) 
		{ 
			DESCryptoServiceProvider des = new DESCryptoServiceProvider(); 
			int len; 
			len=Text.Length/2; 
			byte[] inputByteArray = new byte[len]; 
			int x,i; 
			for(x=0;x<len;x++) 
			{ 
				i = Convert.ToInt32(Text.Substring(x * 2, 2), 16); 
				inputByteArray[x]=(byte)i; 
			} 
			des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8)); 
			des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8)); 
			System.IO.MemoryStream ms=new System.IO.MemoryStream(); 
			CryptoStream cs=new CryptoStream(ms,des.CreateDecryptor(),CryptoStreamMode.Write); 
			cs.Write(inputByteArray,0,inputByteArray.Length); 
			cs.FlushFinalBlock(); 
			return Encoding.Default.GetString(ms.ToArray()); 
		}
        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string BinkEncrypt(string text, string Key = "coos")
        {
            try
            {
                string lstOneKey = Key.Substring(Key.Length - 1, 1).ToLower();
                string lstTowKey = Key.Substring(Key.Length - 2, 1).ToLower();
                text = IsJorO((int)lstTowKey.ToCharArray()[0]) == true ? string.Format("1{0}", lstOneKey) + text.Insert(text.Length / 2, lstOneKey) : text.Insert(text.Length / 2, lstOneKey) + string.Format("2{0}", lstOneKey);
                text = string.Format("{0}{1}{2}{3}", text.Substring(text.Length - 2, 1), text.Substring(1, text.Length - 3), text.Substring(0, 1), text.Substring(text.Length - 1, 1));
                text = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
                text = IsJorO(text.Length) == true ? text : text + lstOneKey;
                for (int i = 1; i <= (text.Length / 2 - 1); i++)
                {
                    string CatStrOne = text.Substring(0, 2);
                    string CatStrTwo = text.Substring(2, (text.Length - 2));
                    text = CatStrTwo.Insert(i * 2, CatStrOne);
                }
            }
            catch (Exception ex)
            {
                text = ex.Message;
            }
            return text;
        }
        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string BinkDecrypt(string text, string Key = "coos")
        {
            try
            {
                string lstOneKey = Key.Substring(Key.Length - 1, 1).ToLower();
                string lstTowKey = Key.Substring(Key.Length - 2, 1).ToLower();
                for (int i = 1; i <= (text.Length / 2 - 1); i++)
                {
                    string LastKey = text.Substring((text.Length - (i * 2)), 2);
                    text = string.Format("{0}{1}", LastKey, text.Remove(text.Length - (i * 2), 2));
                }
                text = IsJorO(text.Length) == true ? text : text.Substring(0, text.Length - 1);
                text = Encoding.UTF8.GetString(Convert.FromBase64String(text));
                text = string.Format("{0}{1}{2}{3}", text.Substring(text.Length - 2, 1), text.Substring(1, text.Length - 3), text.Substring(0, 1), text.Substring(text.Length - 1, 1));
                text = (IsJorO((int)lstTowKey.ToCharArray()[0]) == true ? text.Remove(0, 2) : text.Remove(text.Length - 2, 2));
                text = IsJorO(text.Length) == true ? text.Remove(text.Length / 2 - 1, 1) : text.Remove(text.Length / 2, 1);
            }
            catch (Exception ex)
            {
                text = ex.Message;
            }
            return text;
        }
        /// <summary>
        /// 判断奇数或者偶数
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool IsJorO(int s)
        {
            if (s % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


    }
}
