namespace ObserVR {
    public static class ObserVR_Events {
        public static ObserVR_CustomEvent CustomEvent(string type) {
            return new ObserVR_CustomEvent(type);
        }
    }
}
