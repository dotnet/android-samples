using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace HowsMyTls {
	public class ClientStatus {

		[JsonProperty ("given_cipher_suites")]
		public List<string> CypherSuites { get; set; }

		[JsonProperty("ephemeral_keys_supported")]
		public bool EphemeralKeysSupported { get; set; }

		[JsonProperty ("session_ticket_supported")]
		public bool SessionTicketSupported { get; set; }

		[JsonProperty ("tls_compression_supported")]
		public bool TlsCompressionSupported { get; set; }

		[JsonProperty ("unknown_cipher_suite_supported")]
		public bool UnknownCipherSuiteSupported { get; set; }

		[JsonProperty ("beast_vuln")]
		public bool BeastVuln { get; set; }

		[JsonProperty ("able_to_detect_n_minus_one_splitting")]
		public bool AbleToDetectNMinusOneSplitting { get; set; }

		[JsonProperty ("tls_version")]
		public string TlsVersion { get; set; }

		[JsonProperty ("rating")]
		public string Rating { get; set; }
	}
}

