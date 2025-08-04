using System.Runtime.Serialization;
namespace Pi.WalletService.Domain.Exceptions
{
    [Serializable]
    public class SetTradeDepositException : Exception
    {
        public SetTradeDepositException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public SetTradeDepositException()
        {
        }

        public SetTradeDepositException(string message)
            : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected SetTradeDepositException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class SetTradeWithdrawException : Exception
    {
        public SetTradeWithdrawException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public SetTradeWithdrawException()
        {
        }

        public SetTradeWithdrawException(string message)
            : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected SetTradeWithdrawException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class SetTradeAccountInfoException : Exception
    {
        public SetTradeAccountInfoException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public SetTradeAccountInfoException()
        {
        }

        public SetTradeAccountInfoException(string message) : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected SetTradeAccountInfoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class SetTradeAuthException : Exception
    {
        public SetTradeAuthException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public SetTradeAuthException()
        {
        }

        public SetTradeAuthException(string message)
            : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected SetTradeAuthException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class SetTradeRefreshTokenException : Exception
    {
        public SetTradeRefreshTokenException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public SetTradeRefreshTokenException()
        {
        }

        public SetTradeRefreshTokenException(string message)
            : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected SetTradeRefreshTokenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
