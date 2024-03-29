﻿using MarketPlace.Application.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MarketPlace.Application.Services.Implementations;

public class PasswordHelper : IPasswordHelper
{
    public string EncodePasswordMd5(string password) // Encrypt using MD5
    {
        Byte[] originalBytes;
        Byte[] encodedBytes;

        MD5 md5;

        // compute hash from original pass
        md5 = new MD5CryptoServiceProvider();
        originalBytes = ASCIIEncoding.Default.GetBytes(password);
        encodedBytes = md5.ComputeHash(originalBytes);

        // convert encoded bytes back to a 'readabl' string
        return BitConverter.ToString(encodedBytes);
    }
}
