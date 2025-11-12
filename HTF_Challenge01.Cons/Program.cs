using HTF_Challenge01.Cons;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        string baseUrl = "https://exs-htf-2025.azurewebsites.net";
        string teamKey = "97f4a7df-b18d-46fc-9f42-2b0193bbfabd";

        using HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Team", teamKey);

        var response = await client.GetAsync($"{baseUrl}/api/challenges/sonar?isTest=true");
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<SonarResponse>(jsonResponse);

        if (data == null) return;

        var depthMap = new Dictionary<(int, int), int>();
        foreach (var row in data.SonarHeatmap)
        {
            foreach (var point in row)
            {
                depthMap[(point[0], point[1])] = point[2];
            }
        }

        var start = new Position((int)data.StartPoint[0], (int)data.StartPoint[1], (int)data.StartPoint[2]);
        var end = new Position((int)data.EndPoint[0], (int)data.EndPoint[1], (int)data.EndPoint[2]);

        var path = FindPath(start, end, depthMap);
        if (path == null) return;

        var commands = new List<string>();
        for (int i = 1; i < path.Count; i++)
        {
            var prev = path[i - 1];
            var curr = path[i];
            if (curr.X < prev.X) commands.Add("L");
            else if (curr.X > prev.X) commands.Add("R");
            else if (curr.Y > prev.Y) commands.Add("F");
            else if (curr.Y < prev.Y) commands.Add("B");
        }

        var answer = new { answer = commands.ToArray() };
        var json = JsonSerializer.Serialize(answer);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await client.PostAsync($"{baseUrl}/api/challenges/sonar", content);
        Console.WriteLine(await result.Content.ReadAsStringAsync());
    }


    static List<Position>? FindPath(Position start, Position end, Dictionary<(int, int), int> depthMap)
    {
        var openSet = new PriorityQueue<Node, int>();
        var closedSet = new HashSet<Position>();
        var parent = new Dictionary<Position, Position>();
        var gScore = new Dictionary<Position, int> { [start] = 0 };

        openSet.Enqueue(new Node(start, 0), 0);

        while (openSet.Count > 0)
        {
            var node = openSet.Dequeue();
            var pos = node.Position;

            if (closedSet.Contains(pos)) continue;
            closedSet.Add(pos);

            if (pos.X == end.X && pos.Y == end.Y)
            {
                var path = new List<Position> { pos };
                while (parent.ContainsKey(pos))
                {
                    pos = parent[pos];
                    path.Add(pos);
                }
                path.Reverse();
                return path;
            }

            var moves = new[]
            {
                new Position(pos.X - 1, pos.Y, pos.Z),
                new Position(pos.X + 1, pos.Y, pos.Z),
                new Position(pos.X, pos.Y + 1, pos.Z),
                new Position(pos.X, pos.Y - 1, pos.Z)
            };

            foreach (var next in moves)
            {
                if (next.X < 0 || next.X >= 50 || next.Y < 0 || next.Y >= 50) continue;
                if (!depthMap.TryGetValue((next.X, next.Y), out int floor)) continue;
                if (next.Z < floor) continue;
                if (closedSet.Contains(next)) continue;

                int newCost = gScore[pos] + 1;
                if (!gScore.ContainsKey(next) || newCost < gScore[next])
                {
                    gScore[next] = newCost;
                    parent[next] = pos;
                    int h = Math.Abs(next.X - end.X) + Math.Abs(next.Y - end.Y);
                    int f = newCost + h;
                    openSet.Enqueue(new Node(next, f), f);
                }
            }
        }

        return null;
    }
}
