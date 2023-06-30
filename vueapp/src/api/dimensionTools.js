export function dividirDimensao(total, numFolhas) {
    var tamanhoBase = Math.floor(total / numFolhas);
    var restante = total % numFolhas;
    
    var folhas = [];
    
    for(var i = 0; i < numFolhas; i++) {
        if(i < restante) {
            folhas.push(tamanhoBase + 1);
        } else {
            folhas.push(tamanhoBase);
        }
    }
    
    return folhas;
}

export function calcularTamanhoFolhas(tamanhoTotal, percentagens) {
    let folhas = [];
    
    percentagens.forEach(percentagem => {
        let tamanhoFolha = Math.floor(tamanhoTotal * (percentagem / 100));
        folhas.push(tamanhoFolha);
    });
    
    // correção de arredondamento
    let somaTamanhos = folhas.reduce((a, b) => a + b, 0);
    let diferenca = tamanhoTotal - somaTamanhos;
    
    if (diferenca > 0) {
        folhas[folhas.length - 1] += diferenca;
    }
    
    return folhas;
}

export default
{
    dividirDimensao,
    calcularTamanhoFolhas
}