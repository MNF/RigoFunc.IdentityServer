using System;
using System.IO;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;

namespace RigoFunc.IdentityServer {
    public class PersistedGrantSerializer : IDataSerializer<PersistedGrant> {
        private const int GrantVersion = 1;

        public PersistedGrant Deserialize(byte[] data) {
            using (var memory = new MemoryStream(data)) {
                using (var reader = new BinaryReader(memory)) {
                    return Read(reader);
                }
            }
        }

        public byte[] Serialize(PersistedGrant grant) {
            using (var memory = new MemoryStream()) {
                using (var writer = new BinaryWriter(memory)) {
                    Write(writer, grant);
                }

                return memory.ToArray();
            }
        }

        protected virtual void Write(BinaryWriter writer, PersistedGrant grant) {
            if (writer == null) {
                throw new ArgumentNullException(nameof(writer));
            }

            if (grant == null) {
                throw new ArgumentNullException(nameof(grant));
            }

            // control the binary compatibility
            writer.Write(GrantVersion);

            writer.Write(grant.Key ?? string.Empty);
            writer.Write(grant.SubjectId ?? string.Empty);
            writer.Write(grant.Type ?? string.Empty);
            writer.Write(grant.ClientId ?? string.Empty);
            writer.Write(grant.Data ?? string.Empty);
            writer.Write(grant.CreationTime.Ticks);
            writer.Write(grant.Expiration.Ticks);
        }

        protected virtual PersistedGrant Read(BinaryReader reader) {
            if (reader == null) {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.ReadInt32() != GrantVersion) {
                return null;
            }

            return new PersistedGrant {
                Key = reader.ReadString(),
                SubjectId = reader.ReadString(),
                Type = reader.ReadString(),
                ClientId = reader.ReadString(),
                Data = reader.ReadString(),
                CreationTime = new DateTime(reader.ReadInt64()),
                Expiration = new DateTime(reader.ReadInt64())
            };
        }
    }
}
