using AutoMapper;
using SpecialPlugin.Project.OldDapperDemo;

namespace SpecialPlugin.Project.ReplaceReplaceController
{
    public class NNTest : ITest
    {
        private readonly IMapper _mapper;

        public NNTest(IMapper mapper)
        {
            _mapper = mapper; //V10.0.0.0 是New中的版本所以Old会转换失败
        }

        public string Get()
        {
            //OldDapperDemo.Models.BookTag bookTag = new OldDapperDemo.Models.BookTag()
            //{
            //    LibraryCode = "LibraryCode",
            //    Barcode = "Barcode",
            //    Uid = "Uid"
            //};

            //return _mapper.Map<OldDapperDemo.Dtos.BookTagDto>(bookTag).Barcode;

            NewDapperDemo.Models.BookTag bookTag = new NewDapperDemo.Models.BookTag()
            {
                LibraryCode = "LibraryCode",
                Barcode = "Barcode",
                Uid = "Uid"
            };

            return _mapper.Map<NewDapperDemo.Dtos.BookTagDto>(bookTag).Uid;
        }
    }
}
