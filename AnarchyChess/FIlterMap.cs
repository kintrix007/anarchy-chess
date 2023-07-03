namespace AnarchyChess {
    public static class IEnumerableUtils {
        public static IEnumerable<U> SelectNotNull<T, U>(this IEnumerable<T> list, Func<T, U?> f) {
            return list.Select(f).Where(x => x != null).Select(x => x!);
        }
    }
}
