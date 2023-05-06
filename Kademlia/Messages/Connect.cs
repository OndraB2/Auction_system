
using System.Security.Cryptography;
using System.Text;

namespace Kademlia
{
    class Connect : Message
    {
        public int Nonce;
        public string Hash;
        public static event EventHandler ?OnReceiveRegistrations;
        public Connect(KademliaNode senderNode, KademliaNode destinationNode) : base(senderNode, destinationNode) {}

        public override byte[] Serialize()
        {
            return Serializer.Serialize<Connect>(this);
        }

        public override void OnReceive()
        {
            // cant test if is for me - it is message for bootstrap node while dont know his id
            Console.WriteLine("connect received");
            OnReceiveRegistrations?.Invoke(this, new EventArgs());
        }

        public void CaptchaValidation(int difficulty = 3)
        {
            ComputeNonce(difficulty);
        }

        public bool CaptchaCheck(int difficulty = 3)
        {
            string checkHash = GetHash(ToByteArray(this.Nonce));
            if(checkHash == this.Hash)
            {
                string endValue = new string('0', difficulty);
                if(checkHash.StartsWith(endValue))
                    return true;
            }
            return false;
        }

        private void ComputeNonce(int difficulty)
        {
            int nonce = 0;
            string hash = "";
            string endValue = new string('0', difficulty);

            do
            {
                nonce++;
                hash = GetHash(ToByteArray(nonce));
            }while(!hash.StartsWith(endValue));

            this.Hash = hash;
            this.Nonce = nonce;
        }

        private byte[] ToByteArray(int nonce)
        {
            using(MemoryStream stream = new MemoryStream())
            using(BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(this.SenderNode.ToByteArray());
                //writer.Write(this.DestinationNode.ToByteArray());
                writer.Write(nonce);

                return stream.ToArray();
            }
        }

        private static readonly SHA256 sha256 = SHA256.Create();
        private static string GetHash(byte[] data)
        {
            byte[] hashData = sha256.ComputeHash(data);

            var sBuilder = new StringBuilder();
            // format each as a hexadecimal string.
            for (int i = 0; i < hashData.Length; i++)
            {
                sBuilder.Append(hashData[i].ToString("x2"));
            }
            return sBuilder.ToString();
        } 
    }
}