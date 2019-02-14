[TableName(Constants.TableSurfix + "Comments"),
        ExplicitColumns,
        PrimaryKey("id", autoIncrement = true)]
    public class CommentDTO : IComment
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("pageId")]
        public int PageId { get; set; }

        [Column("isApproved")]
        public bool IsApproved { get; set; }

        [Column("fullName")]
        public string FullName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

        [Column("isTrashed")]
        public bool IsTrashed { get; set; }

        [Column("isSpam")]
        public bool IsSpam { get; set; }
    }