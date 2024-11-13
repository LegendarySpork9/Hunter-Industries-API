namespace HunterIndustriesAPI.Objects
{
    /// <summary>
    /// </summary>
    public class ChangeRecord
    {
        /// <summary>
        /// Id of the record.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The endpoint that was called.
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        /// The field which was updated.
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// The previous value of the field.
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// The new value of the field.
        /// </summary>
        public string NewValue { get; set; }
    }
}