﻿namespace Estoque.Configuration
{
    public class KafkaOptions
    {
        public string BootStrapServer { get; set; }
        public string SaslUsername { get; set; }
        public string SaslPassword { get; set; }
    }
}
