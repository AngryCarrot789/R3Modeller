namespace R3Modeller.Core.Engine.Properties {
    public readonly struct TransferValueCommand {
        public readonly R3Object Owner;
        public readonly R3Property Property;

        public TransferValueCommand(R3Object owner, R3Property property) {
            this.Owner = owner;
            this.Property = property;
        }
    }
}