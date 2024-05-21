using System.Drawing;
public class FingerprintOwner{
    public Bitmap image;
    public string path;
    public string nama;

    public FingerprintOwner(Bitmap image, string path, string name){
        this.image = image;
        this.path = path;
        this.nama = name;
    }
}