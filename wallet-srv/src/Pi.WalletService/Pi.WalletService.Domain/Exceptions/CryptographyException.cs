using System.Runtime.Serialization;
namespace Pi.WalletService.Domain.Exceptions;

public class CryptographyException
{
    [Serializable]
    public class RsaEncryptionException : Exception
    {
        public RsaEncryptionException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public RsaEncryptionException()
        {
        }

        public RsaEncryptionException(string message)
            : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected RsaEncryptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class RsaDecryptionException : Exception
    {
        public RsaDecryptionException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public RsaDecryptionException()
        {
        }

        public RsaDecryptionException(string message)
            : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected RsaDecryptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class AesEncryptionException : Exception
    {
        public AesEncryptionException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public AesEncryptionException()
        {
        }

        public AesEncryptionException(string message)
            : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected AesEncryptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class AesDecryptionException : Exception
    {
        public AesDecryptionException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public AesDecryptionException()
        {
        }

        public AesDecryptionException(string message)
            : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected AesDecryptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}