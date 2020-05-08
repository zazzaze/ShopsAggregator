using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShopsAggregatorWebApi.Models;

namespace ShopsAggregatorWebApi.Models
{
    [Table("posts")]
    public class Post
    {
        private const String PostsPhotoDirectory = "../images";
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Int32 Id { get; set; }
        [Column("creatorid")]
        public Int32 CreatorId { get; set; }
        [Column("info")]
        public String Info { get; set; }

        
        public String ImagePath => Id + "photo.jpeg";
        
        [Column("comments")]
        public List<Int32> Comments { get; set; } = new List<Int32>();

        [Column("likers")]
        public List<Int32> Likers { get; set; } = new List<Int32>();
        
        public void CreatePostFromForm(CreatePostForm form)
        {
            this.CreatorId = form.CreatorId;
            this.Info = form.Info;
            AddPostImage(form);
        }

        public Post()
        {
            
        }
        
        private void AddPostImage(CreatePostForm form)
        {
            if (!Directory.Exists(PostsPhotoDirectory))
                Directory.CreateDirectory(PostsPhotoDirectory);
            using (FileStream fs = new FileStream(PostsPhotoDirectory + $"/{this.Id}photo.jpeg",
                FileMode.OpenOrCreate,
                FileAccess.Write))
            {
                foreach (Int32 imageByte in form.ImageBytes)
                {
                    fs.WriteByte((Byte) imageByte);
                }
            }
        }
    }
}