using System.Runtime.Serialization;

namespace System.FunctionalExtensions
{
    [Serializable]
    public class EitherValueAccessException : Exception
    {
        public EitherValueAccessException() { }
        public EitherValueAccessException(string message) : base(message) { }
        public EitherValueAccessException(string message, Exception innerException) : base(message, innerException) { }
        protected EitherValueAccessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// The Either type represents values with two possibilities: a value of type <c>Either{TL, TR}</c> is
    /// either <c>Left{TL}</c> or <c>Right{TR}</c>.
    /// <para>
    /// The Either type is sometimes used to represent a value which is either correct or an error;
    /// by convention, the <c>Left</c> is used to hold an error value and the <c>Right</c> is used to hold a correct value
    /// (mnemonic: "right" also means "correct"). 
    /// </para>
    /// </summary>
    /// <typeparam name="TL">Type of the left value</typeparam>
    /// <typeparam name="TR">Type of the right value</typeparam>
    public struct Either<TL, TR> : IEquatable<Either<TL, TR>>
    {
        private readonly TL _left;
        private readonly TR _right;
        private readonly bool _isLeft;

        /// <summary>
        /// Don't use!
        /// (Initialize an <c>Left</c> Either.)
        /// </summary>
        public Either(TL leftValue)
        {
            _left = leftValue;
            _right = default(TR);
            _isLeft = true;
        }

        /// <summary>
        /// Don't use!
        /// (Initialize an <c>Right</c> Either.)
        /// </summary>
        public Either(TR rightValue)
        {
            _right = rightValue;
            _left = default(TL);
            _isLeft = false;
        }

        /// <summary>
        /// <c>true</c> if the option is a <c>Left</c>.
        /// </summary>
        public bool IsLeft { get { return _isLeft; } }

        /// <summary>
        /// <c>true</c> if the option is a <c>Right</c>.
        /// </summary>
        public bool IsRight { get { return !_isLeft; } }

        /// <summary>
        /// The value of an <c>Left</c>.
        /// <para>Throws an exception on an <c>Right</c>. Don't catch this exception!</para>
        /// </summary>
        public TL Left
        {
            get
            {
                if (_isLeft)
                {
                    return _left;
                }
                throw new EitherValueAccessException("Tried to get the left value from a right either.");
            }
        }

        /// <summary>
        /// The value of an <c>Right</c>.
        /// <para>Throws an exception on an <c>Left</c>. Don't catch this exception!</para>
        /// </summary>
        public TR Right
        {
            get
            {
                if (!_isLeft)
                {
                    return _right;
                }
                throw new EitherValueAccessException("Tried to get the right value from a left either.");
            }
        }

        public override string ToString()
        {
            var tleft = typeof(TL).ToString();
            var tright = typeof(TR).ToString();
            return _isLeft ? string.Format("Either<{0}, {1}>.Left({2})", tleft, tright, _left)
                           : string.Format("Either<{0}, {1}>.Right({2})", tleft, tright, _right);
        }

        public bool Equals(Either<TL, TR> other)
        {
            return other._isLeft.Equals(_isLeft) && (_isLeft ? Equals(other._left, _left) : Equals(other._right, _right));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Either<TL, TR>)) return false;
            return Equals((Either<TL, TR>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_isLeft.GetHashCode() * 397) ^ (_isLeft ? _left.GetHashCode() : _right.GetHashCode());
            }
        }

        public static bool operator ==(Either<TL, TR> left, Either<TL, TR> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Either<TL, TR> left, Either<TL, TR> right)
        {
            return !left.Equals(right);
        }
    }
}
