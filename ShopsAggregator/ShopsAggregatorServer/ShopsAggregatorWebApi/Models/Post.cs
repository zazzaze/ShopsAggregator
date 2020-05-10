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
    /// <summary>
    /// Тип записи, которую создает пользователь-продавец
    /// </summary>
    [Table("posts")]
    public class Post
    {
        /// <summary>
        /// Путь для сохранения картинок.
        /// </summary>
        private const String PostsPhotoDirectory = "../images";
        /// <summary>
        /// Id записи.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Int32 Id { get; set; }
        /// <summary>
        /// Id создателя записи - пользователя-продавца.
        /// </summary>
        [Column("creatorid")]
        public Int32 CreatorId { get; set; }
        /// <summary>
        /// Информации о записи.
        /// </summary>
        [Column("info")]
        public String Info { get; set; }
        
        /// <summary>
        /// Id пользователей-покупателей, которым понравилась эта запись.
        /// </summary>
        [Column("likers")]
        public List<Int32> Likers { get; set; } = new List<Int32>();
        
        /// <summary>
        /// Ставит полям значения из формы и вызывает метод добавления фотографии.
        /// </summary>
        /// <param name="form">Форма создания записи.</param>
        public void CreatePostFromForm(CreatePostForm form)
        {
            this.CreatorId = form.CreatorId;
            this.Info = form.Info;
            AddPostImage(form);
        }

        /// <summary>
        /// Пустой конструктор.
        /// </summary>
        public Post()
        {
            
        }
        
        /// <summary>
        /// Добавляет фотографию для записи из формы.
        /// </summary>
        /// <param name="form">Форма записи.</param>
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