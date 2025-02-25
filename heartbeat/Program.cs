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
        for (var i = 0; i < number; i++)
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
        List<List<Vector2>>? circlePositions = null;
        float v = 0.0f;
        
        while (!Raylib.WindowShouldClose())
        {
            int circles = 100;
            float dt = Raylib.GetFrameTime();
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            Raylib.DrawText("Heartbeat", 12, 12, 20, Color.Black);
            
            oldCircles ??= GenerateCircles(circles);
            newCircles ??= GenerateCircles(circles);
            circlePositions ??= Enumerable.Range(0, circles).Select(i => new List<Vector2>()).ToList();

            for (int i = 0; i < oldCircles.Count; ++i)
            {
                Vector2 oldCircle = oldCircles[i];
                Vector2 newCircle = newCircles[i];
                Vector2 currentCircle = Vector2.Lerp(oldCircle, newCircle, v);
                DrawCircleTrail(circlePositions, i);
                DrawCircle(currentCircle);
                AddCirclePosition(circlePositions, currentCircle, i);
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

    private static void AddCirclePosition(List<List<Vector2>> circlePositions, Vector2 c, int i)
    {
        int max = 100;
        circlePositions[i].Add(c);
        if (circlePositions[i].Count > max)
        {
            circlePositions[i].RemoveAt(0);
        }
    }

    private static Color GetColorForPosition(Vector2 p)
    {
        int w = Raylib.GetScreenWidth();
        int h = Raylib.GetScreenHeight();
        byte r = (byte)((p.X / w) * 255.0f);
        byte g = (byte)((p.Y / h) * 255.0f);
        byte b = (byte)(MathF.Sin((float)Raylib.GetTime()) * 255.0f);
        return new Color(r, g, b);
    }

    private static void DrawCircle(Vector2 c)
    {
        float radius = 8.0f;
        Color current = GetColorForPosition(c);
        Raylib.DrawCircle((int)c.X, (int)c.Y, radius, current);
    }

    private static void DrawCircleTrail(List<List<Vector2>> circlePositions, int i)
    {
        if (circlePositions[i].Count == 0)
        {
            return;
        }
        
        for (int c = 1; c < circlePositions[i].Count; ++c)
        {
            var a = circlePositions[i][c - 1];
            var b = circlePositions[i][c];
            var l = Raylib.ColorLerp(GetColorForPosition(a), GetColorForPosition(b), 0.5f);
            var alpha = ((float)c / circlePositions[i].Count);
            var t = 16.0f * alpha;
            var m = Raylib.ColorAlpha(l, alpha);
            Raylib.DrawLineEx(a, b, t, m);
        }
    }
}