public abstract class Generator{
        protected Config config;
        protected Weltschmerz weltschmerz;
        public Generator(Weltschmerz weltschmerz, Config config){
            this.config = config;
            this.weltschmerz = weltschmerz;
            Update();
        }

        public abstract void Update();
        public abstract void ChangeConfig(Config config);
}
