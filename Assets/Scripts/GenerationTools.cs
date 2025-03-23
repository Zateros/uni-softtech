using UnityEngine;

public class GenerationTools 
{

    private int[] perms;
    private int seed;

    public GenerationTools() { Reseed(); }
    public GenerationTools(int seed) { Reseed(seed); }

    public void Reseed() => Reseed(Random.Range(int.MinValue, int.MaxValue)); 

    public void Reseed(int seed) {
        this.seed = seed;
        InitNoise();
    }

    // Perlin Noise implementation
    // Source: https://adrianb.io/2014/08/09/perlinnoise.html

    public void InitNoise()
    {
        // Create permutation list (double for performance)
        perms = new int[512];
        Random.InitState(seed);

        // Create base list from 0 to 255 in ascending order
        for (int i = 0; i < 256; ++i) perms[i] = i;

        // Shuffle list
        for (int i = 255; i > 1; --i)
        {
            int k = Random.Range(0, i);
            (perms[k], perms[i]) = (perms[i], perms[k]);
        }

        // Copy shuffled list to the duplicated part
        for (int i = 0; i < 256; ++i) perms[i + 256] = perms[i];
    }

    // Fade function from Ken Perlin (reordered)
    private float PerlinFade(float t) => t * t * t * (t * (t * 6f - 15f) + 10f);

    // Dot product of gradient vectors x and y
    private float PerlinGradient(int hash, float x, float y) => ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y);

    public float PerlinNoise(float x, float y)
    {

        // "Unit rectangle" - P(x, y) is the point
        // eg. P([5] < x < [6] ,[1] < y < [5] )
        // 0------1------2------3
        // |      |      |      |
        // |      | P    |      |
        // 4------5------6------7
        // |      |      |      |
        // |      |      |      |
        // 8------9------10-----11
        // |      |      |      |
        // |      |      |      |
        // 12-----13-----14-----15

        int xx = Mathf.FloorToInt(x) & 0xff;
        int yy = Mathf.FloorToInt(y) & 0xff;

        // Floating part
        x -= Mathf.FloorToInt(x);
        y -= Mathf.FloorToInt(y);

        // Easing towards integer values, thus smoothing the output
        float u = PerlinFade(x);
        float v = PerlinFade(y);

        // Hashing
        int A = perms[xx] + yy & 0xff;
        int B = perms[xx + 1] + yy & 0xff;


        return Mathf.Lerp(
            Mathf.Lerp(
                PerlinGradient(perms[A], x, y),
                PerlinGradient(perms[B], x - 1, y),
                u
            ),
            Mathf.Lerp(
                PerlinGradient(perms[A + 1], x, y - 1),
                PerlinGradient(perms[B + 1], x - 1, y - 1),
                u
            ),
            v
        );
    }

    public float PerlinNoise(Vector2 coord) => PerlinNoise(coord.x, coord.y);

    public float Fbm(Vector2 coord, int octave)
    {
        var f = 0.0f;
        var w = 0.5f;
        for (var i = 0; i < octave; i++)
        {
            f += w * PerlinNoise(coord);
            coord *= 2.0f;
            w *= 0.5f;
        }
        return f;
    }

    public float Fbm(float x, float y, int octave) => Fbm(new Vector2(x, y), octave);

    // ---------------------------------------

    // Gaussian blur

    public static Texture2D GaussianBlur(Color[] pixels, int width, int height, int kernelSize, float sigma) {
        Texture2D tex = new Texture2D(width, height);
        tex.SetPixels(pixels);
        return GaussianBlur(tex,kernelSize, sigma);
    }

    public static Texture2D GaussianBlur(Texture2D texture, int kernelSize, float sigma) {
        float[,] kernel = GenerateKernel(kernelSize, sigma);
        
        Texture2D blurred = new(texture.width, texture.height);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color blurredPixel = ApplyKernel(texture, x,y,kernel);
                blurred.SetPixel(x, y, blurredPixel);
            }
        }

        blurred.Apply();
        return blurred;
    }

    private static Color ApplyKernel(Texture2D texture, int x, int y, float[,] kernel)
    {
        int kernelSize = kernel.GetLength(0);
        int offset = kernelSize/2;
        Color result = Color.black;

        for (int ky = -offset; ky <= offset; ky++)
        {
            for (int kx = -offset; kx <= offset; kx++)
            {
                int px = Mathf.Clamp(x + kx, 0, texture.width-1);
                int py = Mathf.Clamp(y + ky, 0, texture.height-1);

                Color pixel = texture.GetPixel(px,py);

                result += pixel * kernel[ky+offset,kx+offset];

                // Bypass blur on alpha channel (would always be 1f, regardless)
                result.a = pixel.a;
            }
        }

        return result;
    }

    private static float[,] GenerateKernel(int size, float sigma) {
        float[,] kernel = new float[size,size];
        float sum = 0f;
        int offset = size / 2 ;

        for (int y = -offset; y <= offset; y++)
        {
            for (int x = -offset; x <= offset; x++)
            {
                sum += kernel[y+offset, x + offset] = 
                    //https://en.wikipedia.org/wiki/Gaussian_blur#Mathematics
                    Mathf.Exp(
                        -(x*x + y*y) 
                              / 
                        (2f*sigma*sigma)
                    )
                           /
                    (2f*Mathf.PI*sigma*sigma);
            }
        }

        // Normalization
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                kernel[y,x] /= sum;
            }
        }

        return kernel;
    }

    // ---------------------------------------

}
