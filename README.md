![banner](https://github.com/Cocoa2219/EasyRiroSchool/blob/master/docs/banner.png)

# EasyRiroSchool

**EasyRiroSchool**는 ~~광주대동고등학교~~ 리로스쿨의 데이터를 쉽게 가져올 수 있는 .NET 라이브러리입니다. 리로스쿨은 ~~(왜 그런지 모르겠지만)~~ Server Side Rendering(서버에서 HTML을 완성하여 클라이언트에게 전송하는 방식)을 사용하기 때문에, 구조화된 데이터를 쉽게 얻어낼 수 없습니다. **EasyRiroSchool**은 이러한 리로스쿨의 렌더링된 HTML을 스크래핑하여 데이터를 추출합니다.

## 필요 사항
- .NET 6.0 이상

## 설치 방법
```bash
dotnet add package IDidntUploadedPackageYet
```
## 사용 방법
```csharp
using EasyRiroSchool;
using EasyRiroSchool.Models;
using System.Threading.Tasks;
    
class Program
{
    static async Task Main(string[] args)
    {
        // DoSomething();
    }
}
```

## 사용된 라이브러리
- [HTML Agility Pack](https://html-agility-pack.net/)
- [System.Text.Json](https://learn.microsoft.com/dotnet/api/system.text.json)