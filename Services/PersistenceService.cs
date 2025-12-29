using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using SportsClub.Models;

namespace SportsClub.Services
{
    public static class PersistenceService
    {
        // Requirement notes:
        // - implements text, binary and XML persistence (Requirement 16)
        // - provides Save/Load for app state
        public static void SaveAsText(string path, List<Member> members)
        {
            using var sw = new StreamWriter(path, false, Encoding.UTF8);
            foreach (var m in members)
            {
                // Format: Id|FullName|Registered|IsActive|SubscriptionType
                sw.WriteLine($"{m.Id}|{m.FullName}|{m.Registered:O}|{m.IsActive}|{m.Subscription?.Type}");
            }
        }

        public static List<Member> LoadFromText(string path)
        {
            var list = new List<Member>();
            if (!File.Exists(path)) return list;
            foreach (var line in File.ReadAllLines(path, Encoding.UTF8))
            {
                var parts = line.Split('|');
                if (parts.Length < 4) continue;
                if (!Guid.TryParse(parts[0], out var gid)) gid = Guid.NewGuid();
                var name = parts[1];
                _ = DateTime.Now;
                DateTime reg;
                DateTime.TryParse(parts[2], null, System.Globalization.DateTimeStyles.RoundtripKind, out reg);
                // New format: id|name|reg|isActive|sub?
                var subType = parts.Length > 4 ? parts[4] : string.Empty;

                var m = new Member(name)
                {
                    Id = gid,
                    Registered = reg,
                    IsActive = !bool.TryParse(parts.Length > 3 ? parts[3] : "True", out var a) || a
                };
                if (!string.IsNullOrEmpty(subType)) m.Subscription = subType.Contains("Premium") ? new PremiumMembership() as Membership : new BasicMembership();
                list.Add(m);
            }
            return list;
        }

        public static void SaveAsBinary(string path, List<Member> members)
        {
            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            using var bw = new BinaryWriter(fs, Encoding.UTF8);
            bw.Write(members.Count);
            foreach (var m in members)
            {
                // Binary format: Id, FullName, Registered, IsActive, SubscriptionType
                bw.Write(m.Id.ToString());
                bw.Write(m.FullName ?? "");
                bw.Write(m.Registered.ToBinary());
                bw.Write(m.IsActive);
                bw.Write(m.Subscription?.Type ?? "");
            }
        }

        public static List<Member> LoadFromBinary(string path)
        {
            var list = new List<Member>();
            if (!File.Exists(path)) return list;
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs, Encoding.UTF8);
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var id = br.ReadString();
                var name = br.ReadString();
                var reg = DateTime.FromBinary(br.ReadInt64());
                var isActive = br.ReadBoolean();
                var subType = br.ReadString();
                var m = new Member(name)
                {
                    Id = Guid.TryParse(id, out var parsed) ? parsed : Guid.NewGuid(),
                    Registered = reg,
                    IsActive = isActive
                };
                if (!string.IsNullOrEmpty(subType)) m.Subscription = subType.Contains("Premium") ? new PremiumMembership() as Membership : new BasicMembership();
                list.Add(m);
            }
            return list;
        }

        public static void SaveAsXml(string path, List<Member> members)
        {
            var xs = new XmlSerializer(typeof(List<Member>));
            using var fs = new FileStream(path, FileMode.Create);
            xs.Serialize(fs, members);
        }

        public static List<Member> LoadFromXml(string path)
        {
            var xs = new XmlSerializer(typeof(List<Member>));
            if (!File.Exists(path)) return [];
            using var fs = new FileStream(path, FileMode.Open);
            return (List<Member>)xs.Deserialize(fs)!;
        }

        public class AppState
        {
            public List<Member>? Members { get; set; }
            public List<Trainer>? Trainers { get; set; }
            public List<TrainingSession>? Sessions { get; set; }
            public List<Facility>? Facilities { get; set; }
        }

        public static void SaveStateXml(string path, List<Member> members, List<Trainer> trainers, List<TrainingSession> sessions, List<Facility> facilities)
        {
            var xs = new XmlSerializer(typeof(AppState));
            using var fs = new FileStream(path, FileMode.Create);
            var state = new AppState { Members = members, Trainers = trainers, Sessions = sessions, Facilities = facilities };
            xs.Serialize(fs, state);
        }

        public static AppState LoadStateXml(string path)
        {
            var xs = new XmlSerializer(typeof(AppState));
            if (!File.Exists(path)) return new AppState { Members = [], Trainers = [], Sessions = [], Facilities = [] };
            using var fs = new FileStream(path, FileMode.Open);
            return (AppState)xs.Deserialize(fs)!;
        }
    }
}
