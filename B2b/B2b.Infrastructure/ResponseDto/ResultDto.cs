namespace B2b.Infrastructure.ResponseDto
{
    public  class ResultDto<T>
    {
        public bool State { get; set; }
        public string Message { get; set; }
        public List<T> List { get; set; }
        public T Data { get; set; }
    }
}
