#!/usr/bin/env dotnet script
﻿#r "nuget: System.Runtime, 4.3.1"

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Encodings.Web;

var actionsPath = Path.Combine(Directory.GetCurrentDirectory(), "Nationalities.json");

if (!File.Exists(actionsPath))
{
    Console.Error.WriteLine($"Actions.json not found at path: {actionsPath}");
    Environment.Exit(1);
}

var json = File.ReadAllText(actionsPath);
var root = JsonNode.Parse(json) as JsonArray;

if (root is null)
{
    Console.Error.WriteLine("Failed to parse Actions.json as a JSON array.");
    Environment.Exit(1);
}

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};

var count = 0;

foreach (var item in root)
{
    if (item is JsonObject obj)
    {
        obj["Id"] = Guid.CreateVersion7().ToString();
        count++;
    }
}

File.WriteAllText(actionsPath, root.ToJsonString(options));
Console.WriteLine($"Updated Id for {count} actions in Nationalities.json");
