namespace B2b.Infrastructure.Service
{
    public static class GeneralService
    {

        public static bool DeletedFile(string value)
        {
            try
            {
                value = value.Replace("/uploads/", "");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", value);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
