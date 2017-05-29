using System;
using MiscUtil;
using Utilities;

namespace LibToast
{
    public interface IPosition<out T>
    {
        
    }

    public class Position<T> : IPosition<T>
    {
        public T X
        {
            get;
            set;
        }
        public T Y
        {
            get;
            set;
        }

        public Position(T x, T y)
        {
            X = x;
            Y = y;
        }

        public static Position<T> operator +(Position<T> a, Position<T> b)
        {
            return new Position<T>(Operator.Add(a.X, b.X), Operator.Add(a.Y, b.Y));
        }

        public static Position<T> operator *(Position<T> a, Position<T> b)
        {
            return new Position<T>(Operator.Multiply(a.X, b.X), Operator.Multiply(a.Y, b.Y));
        }

        public static Position<T> operator -(Position<T> a, Position<T> b)
        {
            return new Position<T>(Operator.Subtract(a.X, b.X), Operator.Subtract(a.Y, b.Y));
        }

        public static Position<T> operator /(Position<T> a, Position<T> b)
        {
            return new Position<T>(Operator.Divide(a.X, b.X), Operator.Divide(a.Y, b.Y));
        }

        public static Position<T> operator +(Position<T> a, T b)
        {
            return new Position<T>(Operator.Add(a.X, b), Operator.Add(a.Y, b));
        }

        public static Position<T> operator *(Position<T> a, T b)
        {
            return new Position<T>(Operator.Multiply(a.X, b), Operator.Multiply(a.Y, b));
        }

        public static Position<T> operator -(Position<T> a, T b)
        {
            return new Position<T>(Operator.Subtract(a.X, b), Operator.Subtract(a.Y, b));
        }

        public static Position<T> operator /(Position<T> a, T b)
        {
            return new Position<T>(Operator.Divide(a.X, b), Operator.Divide(a.Y, b));
        }

        public static bool operator ==(Position<T> a, Position<T> b)
        {
            return Operator.Equal(a.X, b.X) && Operator.Equal(a.Y, b.Y);
        }

        public static bool operator !=(Position<T> a, Position<T> b)
        {
            return Operator.NotEqual(a.X, b.X) || Operator.NotEqual(a.Y, b.Y);
        }

        public override bool Equals(object obj)
        {
            return obj.GetType().Equals(GetType()) && this == (Position<T>) obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "{0}<{1}>({2},{3})".SFormat(GetType().Name, typeof(T).Name, X, Y);
        }
    }
}