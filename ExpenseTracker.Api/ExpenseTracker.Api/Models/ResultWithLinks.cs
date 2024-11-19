namespace ExpenseTracker.Api.Models
{
    public class ResultWithLinks<TResult> where TResult : class
    {
        public TResult Data { get; set; }
        public List<Link> Links { get; private set; } = [];

        public ResultWithLinks(TResult data)
        {
            Data = data;
        }

        public void AddLink(Link link)
        {
            ArgumentNullException.ThrowIfNull(link);

            Links.Add(link);
        }
    }
}
