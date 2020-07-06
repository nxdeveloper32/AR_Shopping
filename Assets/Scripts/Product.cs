public class Products
{
    public Product[] product;
    [System.Serializable]
    public struct Product
    {
        public string productId;
        public string productName;
        public string productBrand;
        public float productPrice;
        //public int productSales;
        public string productDescription;
        //public int productStock;
        public string fbxPath;
        public string texturePath;
        public string dialTexturePath;
        public string productImage;

        public Product(string productId,string productName, string productBrand, float productPrice, string productDescription, string fbxPath, string texturePath, string dialTexturePath, string productImage)
        {
            this.productId = productId;
            this.productName = productName;
            this.productBrand = productBrand;
            this.productPrice = productPrice;
            this.productDescription = productDescription;
            this.fbxPath = fbxPath;
            this.texturePath = texturePath;
            this.dialTexturePath = dialTexturePath;
            this.productImage = productImage;
        }
    }
    public Products(Product[] product)
    {
        this.product = product;
    }
}
