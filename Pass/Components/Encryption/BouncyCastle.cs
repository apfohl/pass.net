using System;
using System.IO;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;

namespace Pass.Components.Encryption
{
    /**
    * A simple utility class that encrypts/decrypts public key based
    * encryption large files.
    * <p>
    * To encrypt a file: KeyBasedLargeFileProcessor -e [-a|-ai] fileName publicKeyFile.<br/>
    * If -a is specified the output file will be "ascii-armored".
    * If -i is specified the output file will be have integrity checking added.</p>
    * <p>
    * To decrypt: KeyBasedLargeFileProcessor -d fileName secretKeyFile passPhrase.</p>
    * <p>
    * Note 1: this example will silently overwrite files, nor does it pay any attention to
    * the specification of "_CONSOLE" in the filename. It also expects that a single pass phrase
    * will have been used.</p>
    * <p>
    * Note 2: this example Generates partial packets to encode the file, the output it Generates
    * will not be readable by older PGP products or products that don't support partial packet
    * encoding.</p>
	* <p>
	* Note 3: if an empty file name has been specified in the literal data object contained in the
	* encrypted packet a file with the name filename.out will be generated in the current working directory.</p>
    */
    public static class BouncyCastle
    {
        private static void DecryptFile(
            string inputFileName,
            string keyFileName,
            char[] passwd,
            string defaultFileName)
        {
            using Stream input = File.OpenRead(inputFileName), keyIn = File.OpenRead(keyFileName);
            DecryptFile(input, keyIn, passwd, defaultFileName);
        }

        /**
        * decrypt the passed in message stream
        */
        public static Stream DecryptFile(
            Stream inputStream,
            Stream keyIn,
            char[] passwd,
            string defaultFileName)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);
            Stream fOut = new MemoryStream();

            try
            {
                var pgpF = new PgpObjectFactory(inputStream);
                PgpEncryptedDataList enc;

                var o = pgpF.NextPgpObject();
                //
                // the first object might be a PGP marker packet.
                //
                if (o is PgpEncryptedDataList list)
                {
                    enc = list;
                }
                else
                {
                    enc = (PgpEncryptedDataList) pgpF.NextPgpObject();
                }

                //
                // find the secret key
                //
                PgpPrivateKey sKey = null;
                PgpPublicKeyEncryptedData pbe = null;
                var pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(keyIn));

                foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
                {
                    sKey = PgpExampleUtilities.FindSecretKey(pgpSec, pked.KeyId, passwd);

                    if (sKey != null)
                    {
                        pbe = pked;
                        break;
                    }
                }

                if (sKey == null)
                {
                    throw new ArgumentException("secret key for message not found.");
                }

                var clear = pbe.GetDataStream(sKey);

                var plainFact = new PgpObjectFactory(clear);

                var cData = (PgpCompressedData) plainFact.NextPgpObject();

                var pgpFact = new PgpObjectFactory(cData.GetDataStream());

                var message = pgpFact.NextPgpObject();

                switch (message)
                {
                    case PgpLiteralData ld:
                    {
                        var unc = ld.GetInputStream();
                        Streams.PipeAll(unc, fOut);
                        fOut.Flush();
                        fOut.Seek(0, SeekOrigin.Begin);
                        break;
                    }
                    case PgpOnePassSignatureList:
                        throw new PgpException("encrypted message contains a signed message - not literal data.");
                    default:
                        throw new PgpException("message is not a simple encrypted file - type unknown.");
                }

                if (pbe.IsIntegrityProtected())
                {
                    Console.Error.WriteLine(!pbe.Verify()
                        ? "message failed integrity check"
                        : "message integrity check passed");
                }
                else
                {
                    Console.Error.WriteLine("no message integrity check");
                }
            }
            catch (PgpException e)
            {
                Console.Error.WriteLine(e);

                var underlyingException = e.InnerException;
                if (underlyingException != null)
                {
                    Console.Error.WriteLine(underlyingException.Message);
                    Console.Error.WriteLine(underlyingException.StackTrace);
                }
            }

            return fOut;
        }

        private static void EncryptFile(
            string outputFileName,
            string inputFileName,
            string encKeyFileName,
            bool armor,
            bool withIntegrityCheck)
        {
            var encKey = PgpExampleUtilities.ReadPublicKey(encKeyFileName);

            using Stream output = File.Create(outputFileName);
            EncryptFile(output, inputFileName, encKey, armor, withIntegrityCheck);
        }

        private static void EncryptFile(
            Stream outputStream,
            string fileName,
            PgpPublicKey encKey,
            bool armor,
            bool withIntegrityCheck)
        {
            if (armor)
            {
                outputStream = new ArmoredOutputStream(outputStream);
            }

            try
            {
                var cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5,
                    withIntegrityCheck, new SecureRandom());

                cPk.AddMethod(encKey);

                var cOut = cPk.Open(outputStream, new byte[1 << 16]);

                var comData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);

                PgpUtilities.WriteFileToLiteralData(
                    comData.Open(cOut),
                    PgpLiteralData.Binary,
                    new FileInfo(fileName),
                    new byte[1 << 16]);

                comData.Close();

                cOut.Close();

                if (armor)
                {
                    outputStream.Close();
                }
            }
            catch (PgpException e)
            {
                Console.Error.WriteLine(e);

                var underlyingException = e.InnerException;
                if (underlyingException != null)
                {
                    Console.Error.WriteLine(underlyingException.Message);
                    Console.Error.WriteLine(underlyingException.StackTrace);
                }
            }
        }

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine(
                    "usage: KeyBasedLargeFileProcessor -e|-d [-a|ai] file [secretKeyFile passPhrase|pubKeyFile]");
                return;
            }

            if (args[0].Equals("-e"))
            {
                if (args[1].Equals("-a") || args[1].Equals("-ai") || args[1].Equals("-ia"))
                {
                    EncryptFile(args[2] + ".asc", args[2], args[3], true, (args[1].IndexOf('i') > 0));
                }
                else if (args[1].Equals("-i"))
                {
                    EncryptFile(args[2] + ".bpg", args[2], args[3], false, true);
                }
                else
                {
                    EncryptFile(args[1] + ".bpg", args[1], args[2], false, false);
                }
            }
            else if (args[0].Equals("-d"))
            {
                DecryptFile(args[1], args[2], args[3].ToCharArray(), new FileInfo(args[1]).Name + ".out");
            }
            else
            {
                Console.Error.WriteLine(
                    "usage: KeyBasedLargeFileProcessor -d|-e [-a|ai] file [secretKeyFile passPhrase|pubKeyFile]");
            }
        }
    }
}