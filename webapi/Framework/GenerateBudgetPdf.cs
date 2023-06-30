using System.IO;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using PdfSharp.Snippets.Font;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Fields;

namespace webapi.Framework
{
    public class GenerateBudgetPdf
    {
        public static void GeneratePdf(List<Models.Window> janelas, out MemoryStream stream, int? budgetNumber = null, Models.PersonalDetail? client = null)
        {
            if (PdfSharp.Capabilities.Build.IsCoreBuild && GlobalFontSettings.FontResolver == null)
                GlobalFontSettings.FontResolver = new FailsafeFontResolver();

            // Cria um novo documento PDF
            var document = new Document()
            {
                Info =
                {
                    Title = "Orçamento",
                    Author = "Maksym",
                },
                
            };

            var style = document.Styles[StyleNames.Normal]!;
            style.Font.Name = "Arial";

            // Adiciona uma seção ao documento
            var section = document.AddSection();

            // Configura as margens da página
            section.PageSetup.LeftMargin = "1cm";
            section.PageSetup.RightMargin = "1cm";
            //section.PageSetup.TopMargin = "1cm";
            //section.PageSetup.BottomMargin = "1cm";

            // Adicionar o titulo
            var budgetTitle = new Paragraph();
            budgetTitle.AddFormattedText(string.Format("Orçamento Nº: {0}", budgetNumber), new Font("Segoe UI", 20) { Bold = true });
            budgetTitle.Format.SpaceAfter = "1cm";
            budgetTitle.Format.Alignment = ParagraphAlignment.Center;
            section.Add(budgetTitle);

            // Adicionar os dados do cliente
            if(client != null)
            {
                var table = section.AddTable();
                table.AddColumn("3cm");
                table.AddColumn("16cm");
                table.KeepTogether = true;

                var nameRow = table.AddRow();
                nameRow.Cells[0].AddParagraph().AddFormattedText("Nome:", TextFormat.Bold);
                nameRow.Cells[1].AddParagraph(string.Format("{0}", string.Join(" ", new[] { client.FirstName, client.LastName })));

                var telRow = table.AddRow();
                nameRow.Cells[0].AddParagraph().AddFormattedText("Telemóvel:", TextFormat.Bold);
                nameRow.Cells[1].AddParagraph(string.Format("{0}", client.Phone));

                var emailRow = table.AddRow();
                nameRow.Cells[0].AddParagraph().AddFormattedText("E-mail:", TextFormat.Bold);
                nameRow.Cells[1].AddParagraph(string.Format("{0}", client.Email));

                /*var addressRow = table.AddRow();
                nameRow.Cells[0].AddParagraph().AddFormattedText("Morada:", TextFormat.Bold);
                nameRow.Cells[1].AddParagraph(string.Format("{0}", client...));*/
            }
            
            // Adicionar as janelas
            int windowNumber = 1;
            foreach (var janela in janelas)
            {
                if(janela == null)
                    continue;

                // Adiciona uma tabela à seção
                var table = section.AddTable();
                table.AddColumn("8cm");
                table.AddColumn("11cm");
                table.KeepTogether = true;

                // Desenha o texto - Adiciona um parágrafo à seção (Titulo)
                var titleRow = table.AddRow();
                titleRow.KeepWith = 1;
                titleRow.Cells[0].MergeRight = 1;
                
                var title = new Paragraph();
                title.AddFormattedText(string.Format("Janela {0} [ {1} x {2} ]", windowNumber++, janela.Width, janela.Height), new Font("Segoe UI", 15) { Bold = true });
                title.Format.SpaceBefore = "1cm";
                title.Format.SpaceAfter = "0.5cm";
                title.Format.Alignment = ParagraphAlignment.Center;
                titleRow.Cells[0].Add(title);


                // Adiciona uma linha à tabela
                var row = table.AddRow();

                // Adiciona a imagem da janela à primeira célula da linha
                var schemaJanela = Generate2DWindow.GenerateImageBase64(janela, true, true);
                var image = row.Cells[0].AddImage("base64:" + schemaJanela);
                image.Width = "7.5cm";
                image.Height = "7.5cm";

                // Criar tabela com descrição da janela
                var nestedTable = new Table();
                nestedTable.AddColumn("2.5cm");
                nestedTable.AddColumn("8.5cm");

                var nestedRow = nestedTable.AddRow();
                nestedRow.Cells[0].AddParagraph().AddFormattedText("Dimensão:", TextFormat.Bold);
                nestedTable.AddRow().Cells[1].AddParagraph("Largura   ").AddFormattedText(string.Format("{0} mm", janela.Width), TextFormat.Bold);
                nestedTable.AddRow().Cells[1].AddParagraph("Altura   ").AddFormattedText(string.Format("{0} mm", janela.Height), TextFormat.Bold);

                if(janela.IndorColor != null)
                {
                    nestedRow = nestedTable.AddRow();
                    nestedRow.TopPadding = 5;
                    nestedRow.Cells[0].AddParagraph().AddFormattedText("Cor interior:", TextFormat.Bold);
                    nestedRow.Cells[1].AddParagraph(string.Format("{0} ({1})", janela.IndorColor?.Name, janela.WindowProfile?.Brand?.Name));
                }

                if(janela.OutdorColor != null)
                {
                    nestedRow = nestedTable.AddRow();
                    nestedRow.TopPadding = 5;
                    nestedRow.Cells[0].AddParagraph().AddFormattedText("Cor exterior:", TextFormat.Bold);
                    nestedRow.Cells[1].AddParagraph(string.Format("{0} ({1})", janela.OutdorColor?.Name, janela.WindowProfile?.Brand?.Name));
                }

                nestedRow = nestedTable.AddRow();
                nestedRow.TopPadding = 5;
                nestedRow.Cells[0].AddParagraph().AddFormattedText("Vidro:", TextFormat.Bold);
                nestedRow.Cells[1].AddParagraph(string.Format("duplo ClimatGuard Premium 4-16-4 [{0}]", janela.WindowGlassType?.Name));

                if(janela.WindowProfile != null)
                {
                    nestedRow = nestedTable.AddRow();
                    nestedRow.TopPadding = 5;
                    nestedRow.Cells[0].AddParagraph().AddFormattedText("Perfil:", TextFormat.Bold);
                    nestedRow.Cells[1].AddParagraph(string.Format("{0} ({1})", janela.WindowProfile?.Name, janela.WindowProfile?.Brand?.Name));
                }

                if(!string.IsNullOrWhiteSpace(janela.Description))
                {
                    nestedRow = nestedTable.AddRow();
                    nestedRow.TopPadding = 5;
                    nestedRow.Cells[0].AddParagraph().AddFormattedText("Descrição:", TextFormat.Bold);
                    nestedRow.Cells[1].AddParagraph(janela.Description);
                }

                // Adiciona a tabela com descrição da janela à segunda célula da linha
                row.Cells[1].Elements.Add(nestedTable);
            }

            // Create the primary footer.
            //var footerSection = document.AddSection();
            var footer = section.Footers.Primary;

            // Add content to footer.
            var paragraph = footer.AddParagraph();
            paragraph.Add(new DateField { Format = "yyyy/MM/dd HH:mm:ss" });
            paragraph.Format.Alignment = ParagraphAlignment.Center;


            // Renderiza o documento MigraDoc em um documento PdfSharp
            var renderer = new PdfDocumentRenderer
            {
                // Associate the MigraDoc document with a renderer.
                Document = document,
                PdfDocument = new PdfDocument()
            };
            // Layout and render document to PDF.
            renderer.RenderDocument();


            // Salva o documento em um stream
            stream = new MemoryStream();
            renderer.PdfDocument.Save(stream, false);

            // Retorna o stream como um arquivo PDF
            stream.Position = 0;
            //return new FileStreamResult(stream, "application/pdf");
        }
    }
}
