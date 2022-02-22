using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Wunion.DataAdapter.CodeFirstDemo.Data.Security
{
    /// <summary>
    /// 表示采用 RSA 算法的数据保护.
    /// </summary>
    public class RsaDataProtection : IDataProtection
    {
        private RSACryptoServiceProvider provider;

        /// <summary>
        /// 创建一个 <see cref="RsaDataProtection"/> 的对象实例.
        /// </summary>
        public RsaDataProtection()
        {
            provider = new RSACryptoServiceProvider(2048);
        }

        /// <summary>
        /// 解密指定的文本并返回解密后的结果.
        /// </summary>
        /// <param name="text">待解密的文本.</param>
        /// <returns></returns>
        public string Decrypt(string text)
        {
            byte[] buffer = Convert.FromBase64String(text);
            buffer = provider.Decrypt(buffer, RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// 解指定的字节数组，并返回加密后的结果.
        /// </summary>
        /// <param name="buffer">待解密的字节数组.</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] buffer) => provider.Decrypt(buffer, RSAEncryptionPadding.Pkcs1);

        /// <summary>
        /// 加密指定的文本并返回加密后的结果.
        /// </summary>
        /// <param name="text">要加密的文本.</param>
        /// <returns></returns>
        public string Encrypt(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            buffer = provider.Encrypt(buffer, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 加密指定的字节数组，并返回加密后的结果.
        /// </summary>
        /// <param name="buffer">要加密的字节数组.</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] buffer) => provider.Encrypt(buffer, RSAEncryptionPadding.Pkcs1);

        /// <summary>
        /// 生成并返回 base64 格式的密钥.
        /// </summary>
        /// <returns></returns>
        public string GenerateKey()
        {
            byte[] buffer = provider.ExportRSAPrivateKey();
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 从 base64 字符串中导入 key.
        /// </summary>
        /// <param name="base64">base64 密钥</param>
        public void ImportKey(string base64)
        {
            byte[] buffer = Convert.FromBase64String(base64);
            int byteRead;
            provider.ImportRSAPrivateKey(buffer, out byteRead);
        }

        /// <summary>
        /// 资源对象占用的资源.
        /// </summary>
        /// <param name="disposing">手动则用时为 true，对象终结器调用时则为 false .</param>
        private void Dispose(bool disposing)
        {
            provider?.Dispose();
            if (disposing)
                provider = null;
        }

        /// <summary>
        /// 释放对象占用的资源.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 结象终结器（析构函数）.
        /// </summary>
        ~RsaDataProtection()
        {
            Dispose(false);
        }
    }
}
