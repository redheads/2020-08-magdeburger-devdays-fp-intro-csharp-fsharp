namespace CSharpRefactor
{
    public class InterimResult<T>
    {
        public T Contents { get; }
        public string ErrorText { get; }

        public InterimResult(T contents)
        {
            Contents = contents;
        }
        
        public InterimResult(T contents, string errorText)
        {
            Contents = contents;
            ErrorText = errorText;
        }
            
        public InterimResult(string errorText)
        {
            Contents = default(T);
            ErrorText = errorText;
        }
    }
}