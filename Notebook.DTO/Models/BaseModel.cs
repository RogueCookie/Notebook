namespace Notebook.DTO.Models
{
    /// <summary>
    /// Base class which property will be multiple used
    /// </summary>
    public abstract class BaseModel
    {
        /// <summary>
        /// Id of the record
        /// </summary>
        public long Id { get; set; }
    }
}