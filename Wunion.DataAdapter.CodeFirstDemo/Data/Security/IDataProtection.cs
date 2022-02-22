using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wunion.DataAdapter.CodeFirstDemo.Data.Security
{
    /// <summary>
    /// 实现数据保护的接口.
    /// </summary>
    public interface IDataProtection : IDisposable
    {
        /// <summary>
        /// 生成并返回 base64 格式的密钥.
        /// </summary>
        /// <returns></returns>
        string GenerateKey();

        /// <summary>
        /// 从 base64 字符串中导入 key.
        /// </summary>
        /// <param name="base64">base64 密钥</param>
        void ImportKey(string base64);

        /// <summary>
        /// 加密指定的文本并返回加密后的结果.
        /// </summary>
        /// <param name="text">要加密的文本.</param>
        /// <returns></returns>
        string Encrypt(string text);

        /// <summary>
        /// 加密指定的字节数组，并返回加密后的结果.
        /// </summary>
        /// <param name="buffer">要加密的字节数组.</param>
        /// <returns></returns>
        byte[] Encrypt(byte[] buffer);

        /// <summary>
        /// 解密指定的文本并返回解密后的结果.
        /// </summary>
        /// <param name="text">待解密的文本.</param>
        /// <returns></returns>
        string Decrypt(string text);

        /// <summary>
        /// 解指定的字节数组，并返回加密后的结果.
        /// </summary>
        /// <param name="buffer">待解密的字节数组.</param>
        /// <returns></returns>
        byte[] Decrypt(byte[] buffer);
    }
}
