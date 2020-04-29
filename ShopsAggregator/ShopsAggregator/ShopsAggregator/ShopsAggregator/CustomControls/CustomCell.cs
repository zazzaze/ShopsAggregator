using System;
using Xamarin.Forms;

public partial class PostCell : ViewCell
{

    public PostCell()
    {
        var grid = new Grid();
    }
    private String postImageSource, userIconSource, comment, userName;
        
    private static readonly BindableProperty Username =
        BindableProperty.Create ("UserName", typeof(String), typeof(PostCell), "Noname");
    private static readonly BindableProperty Post =
        BindableProperty.Create ("PostImage", typeof(String), typeof(PostCell), null);
    private static readonly BindableProperty Icon =
        BindableProperty.Create ("Location", typeof(String), typeof(PostCell), null);
    private static readonly BindableProperty UserComment = 
        BindableProperty.Create("Comment", typeof(String), typeof(PostCell), null);
        
    public String Name
    {
        get { return(String)GetValue (Username); }
        set { SetValue (Username, value); }
    }

    public String PostImage
    {
        get { return(String)GetValue (Post); }
        set { SetValue (Post, value); }
    }

    public String UserIcon
    {
        get { return(String)GetValue (Icon); }
        set { SetValue (Icon, value); }
    }

    public String Comment
    {
        get => (String)GetValue (UserComment);
        set => SetValue (UserComment, value);
    }

    protected override void OnBindingContextChanged ()
    {
        base.OnBindingContextChanged ();

        if (BindingContext != null)
        {
            Image image = new Image();
            image.Source = PostImage;
            this.Height= image.HeightRequest;
        }
    }
}