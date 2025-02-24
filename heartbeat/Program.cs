using System.Numerics;
using Raylib_cs;

class Program
{
    static List<Vector2> GenerateCircles(int number)
    {
        float w = Raylib.GetRenderWidth();
        float h = Raylib.GetRenderHeight();

        float minX = 0.0f;
        float maxX = w;

        float minY = 0.0f;
        float maxY = h;
        
        List<Vector2> circles = new List<Vector2>();
        for (var i = 0; i < 100; i++)
        {
            float randomX = Random.Shared.Next((int)minX, (int)maxX);
            float randomY = Random.Shared.Next((int)minY, (int)maxY);
            circles.Add(new Vector2(randomX, randomY));
        }

        return circles;
    }
    
    public static void Main()
    {
        Raylib.InitWindow(800, 480, "Heartbeat");

        List<Vector2>? oldCircles = null;
        List<Vector2>? newCircles = null;
        float v = 0.0f;
        
        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            Raylib.DrawText("Heartbeat", 12, 12, 20, Color.Black);
            
            oldCircles ??= GenerateCircles(10);
            newCircles ??= GenerateCircles(10);

            for (int i = 0; i < oldCircles.Count; ++i)
            {
                Vector2 oldCircle = oldCircles[i];
                Vector2 newCircle = newCircles[i];
                Vector2 currentCircle = Vector2.Lerp(oldCircle, newCircle, v);
                DrawCircle(currentCircle);
                v += MathF.Sin(dt / 100.0f);
            }

            if (v >= 1.0f)
            {
                oldCircles = newCircles;
                newCircles = null;
                v = 0.0f;
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    private static void DrawCircle(Vector2 c)
    {
        float radius = 8.0f;
        byte r = (byte)((int)c.X % 255);
        byte g = (byte)((int)c.Y % 255);
        byte b = (byte)((int)(c.X + c.Y) / 255);
        Color current = new Color(r, g, b);
        Raylib.DrawCircle((int)c.X, (int)c.Y, radius, current);
    }
}