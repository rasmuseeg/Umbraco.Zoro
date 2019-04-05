using System;

namespace Zoro.Domain.Forum
{
    public class ForumComment
    {
        public Guid ThreadId { get; set; }
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        public Guid? ParentId { get; set; }
    }
}
