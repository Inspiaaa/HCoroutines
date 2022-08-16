namespace HCoroutines {
    /// <summary>
    /// This class allows for the hierarchical organisation of coroutines.
    /// The children of the coroutine are implemented as a doubly linked list to allow for
    /// mutation during the iteration.
    /// Therefore each Coroutine keeps a reference to the previous and next sibling,
    /// as well as to the first and last child it has.
    /// </summary>
    public class HCoroutineNode {
        public CoroutineBase firstChild, lastChild;
        public CoroutineBase previousSibling, nextSibling;

        public void AddChild(CoroutineBase coroutine) {
            if (firstChild == null) {
                firstChild = coroutine;
                lastChild = coroutine;
            }
            else {
                lastChild.nextSibling = coroutine;
                coroutine.previousSibling = lastChild;
                lastChild = coroutine;
            }
        }

        public void RemoveChild(CoroutineBase coroutine) {
            if (coroutine.previousSibling != null) {
                coroutine.previousSibling.nextSibling = coroutine.nextSibling;
            }

            if (coroutine.nextSibling != null) {
                coroutine.nextSibling.previousSibling = coroutine.previousSibling;
            }

            if (firstChild == coroutine) {
                firstChild = coroutine.nextSibling;
            }

            if (lastChild == coroutine) {
                lastChild = coroutine.previousSibling;
            }
        }
    }
}
