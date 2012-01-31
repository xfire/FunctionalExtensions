using System.Runtime.Serialization;

namespace System.FunctionalExtensions
{
    [Serializable]
    public class OptionValueAccessException : Exception
    {
        public OptionValueAccessException() { }
        public OptionValueAccessException(string message) : base(message) { }
        public OptionValueAccessException(string message, Exception innerException) : base(message, innerException) { }
        protected OptionValueAccessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class SomeInitializedWithNullException : Exception
    {
        public SomeInitializedWithNullException() { }
        public SomeInitializedWithNullException(string message) : base(message) { }
        public SomeInitializedWithNullException(string message, Exception innerException) : base(message, innerException) { }
        protected SomeInitializedWithNullException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Optional type, which represents either the existence (<c>Some</c>) of a value and the value itself or the
    /// absence (<c>None</c>) of a value.
    /// <para>
    ///   Decision: Option{T} is a struct, so it never can be set to <c>null</c>. Ergo a method returning an Option{T} can
    ///   never return <c>null</c>.
    /// </para>
    /// <para>
    ///   Options should _not_ be created with the Constructors. Instead us one of:
    ///   <list type="bullet">
    ///     <item><description><see cref="Option.Some{TValue}"/></description></item>
    ///     <item><description><see cref="Option.None{TValue}"/></description></item>
    ///     <item><description><see cref="Option.ToOption{T}(System.Nullable{T})"/></description></item>
    ///   </list>
    ///   (Problem is that the constructors of structs can not be made protected or private.)
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of the contained value.</typeparam>
    public struct Option<T> : IEquatable<Option<T>>
    {
        private readonly bool _hasValue;
        private readonly T _value;

        /// <summary>
        /// None for the type {T}.
        /// </summary>
        public static readonly Option<T> None = new Option<T>();

        /// <summary>
        /// Don't use!
        /// </summary>
        public Option(T value)
        {
            if(Equals(value, null))
            {
                throw new SomeInitializedWithNullException("Option.Some can not contain null.");
            }
            _hasValue = true;
            _value = value;
        }

        /// <summary>
        /// <c>true</c> if the option is a <c>Some</c> and contains a value.
        /// </summary>
        public bool IsSome { get { return _hasValue; }}

        /// <summary>
        /// <c>true</c> if the option is a <c>None</c> and does _not_ contain a value.
        /// </summary>
        public bool IsNone { get { return !IsSome; }}

        /// <summary>
        /// The value of a <c>Some</c>.
        /// <para>Throws an exception on a <c>None</c>. Don't catch this exception!</para>
        /// </summary>
        public T Value
        {
            get
            {
                if (_hasValue)
                {
                    return _value;
                }
                throw new OptionValueAccessException("Option.None has no value.");
            }
        }

        public override string ToString()
        {
            return _hasValue ? string.Format("Some({0})", Value) : string.Format("None()");
        }

        public bool Equals(Option<T> other)
        {
            return other._hasValue.Equals(_hasValue) && Equals(other._value, _value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Option<T>)) return false;
            return Equals((Option<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_hasValue.GetHashCode()*397) ^ _value.GetHashCode();
            }
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }
    }
}
