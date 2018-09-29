namespace litclassic
{
    struct Line
    {
        private string line;
        private string titleOld;
        private int strongConnection;
        private int recommendConnection;
        private int explanationStrongConnection;
        private int explanationRecommendConnection;
        private int index;
        // типы тем:
        // "0" - основные произведения
        // "1" - прочие произведения, записки, письма
        // "2" - комментарии, разделы от редакции, биограф.очерки, примечания, приложения
        // "3" - указатели и пр.
        private int themeType;


        public void AddNewPartOfLine(string newLine)
        {
            line += newLine + "\n\r";
        }
        public void AddNewPartOfTitleOld(string newTitle)
        {
            newTitle = newTitle.Replace("\n", " ");
            newTitle = newTitle.Replace("\r", "");
            titleOld += newTitle + ";";
        }
        public void SetStrongConnection(int newStrongConnection)
        {
            strongConnection = newStrongConnection;
        }
        public void SetRecommendConnection(int newRecommendConnection)
        {
            recommendConnection = newRecommendConnection;
        }
        public void SetExplanationStrongConnection(int newExplanationStrongConnection)
        {
            explanationStrongConnection = newExplanationStrongConnection;
        }
        public void SetExplanationRecommendConnection(int newExplanationRecommendConnection)
        {
            explanationRecommendConnection = newExplanationRecommendConnection;
        }
        public void SetIndex(int newIndex)
        {
            index = newIndex;
        }
        public void SetLine(string newLine)
        {
            line = newLine;
        }
        public void SetThemeType (int newThemeType)
        {
            themeType = newThemeType;
        }


        public string GetLine()
        {
            return line;
        }
        public string GetTitleOld()
        {
            return titleOld;
        }
        public int GetStrongConnection()
        {
            return strongConnection;
        }
        public int GetRecommendConnection()
        {
            return recommendConnection;
        }
        public int GetExplanationStrongConnection()
        {
            return explanationStrongConnection;
        }
        public int GetExplanationRecommendConnection()
        {
            return explanationRecommendConnection;
        }
        public int GetIndex()
        {
            return index;
        }
        public int GetThemeType()
        {
            return themeType;
        }
    }
}
