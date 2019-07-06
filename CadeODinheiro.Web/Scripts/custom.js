function OnEndProccessMensagemComRedirecionamento(content) {
    if (content.Sucesso) {
        mensagemRedirecionar(content.Titulo, content.Mensagem, content.Url);
    }
    else if (content.Sucesso == false && content.Mensagem != '') {
        exibirMensagemErro(content.Titulo, content.Mensagem);
    }
    else {
        $('#content').html(content);
    }
}

function OnEndProccessMensagemComReload(content) {
    if (content.Sucesso) {
        exibirMensagemSucessoReload(content.Titulo, content.Mensagem, content.Url);
    }
    else if (content.Sucesso == false && content.Mensagem != '') {
        exibirMensagemErro(content.Titulo, content.Mensagem);
    }
    else {
        $('#content').html(content);
    }
}

function OnEndProccessMensagemSemRedirecionamento(content) {
    if (content.Sucesso) {
        exibirMensagemSucesso(content.Titulo, content.Mensagem);
    }
    else if (content.Sucesso == false && content.Mensagem != '') {
        exibirMensagemErro(content.Titulo, content.Mensagem);
    }
    else {
        $('#content').html(content);
    }
}

function redirecionar(url) {
    location.href = url;
}

function mensagemRedirecionar(titulo, mensagem, url) {
    $.SmartMessageBox({
        title: titulo,
        content: mensagem,
        buttons: '[Ok]'
    }, function (ButtonPressed) {
        if (ButtonPressed === "Ok") {
            redirecionar(url);
        }
    });
    $("#bot1-Msg1").focus();
    }

function exibirMensagemSucessoCallback(titulo, mensagem, callback) {
    $.bigBox({
        title: titulo,
        content: mensagem,
        color: "#739E73",
        //timeout: 4000,
        icon: "fa fa-check",
        //number: "4"
    }, function () {   
        callback();
    });
    //e.preventDefault();
}

function exibirMensagemSucesso(titulo, mensagem) {
    $.bigBox({
        title: titulo,
        content: mensagem,
        color: "#739E73",
        timeout: 4000,
        icon: "fa fa-check",
        //number: "4"
    });
}

function exibirMensagemSucessoReload(titulo, mensagem, url) {
    $.SmartMessageBox({
        title: titulo,
        content: mensagem,
        buttons: '[Ok]'
    }, function (ButtonPressed) {
        if (ButtonPressed === "Ok") {
            location.reload();
        }
    });
    $("#bot1-Msg1").focus();
}

function exibirMensagemSucessoReload2(titulo, mensagem, url) {
    $('body').append('<div id="dialogoMensagemSucesso"></div>');
    $('#dialogoMensagemSucesso').append("<div><strong><p>" + mensagem + "</p></strong></div>");
    $('#dialogoMensagemSucesso').dialog({
        modal: true,
        width: 415,
        heigth: 165,
        title: titulo,
        closeOnEscape: false,
        open: function (event, ui) {
            // Hide close button 
            $(this).parent().children().children(".ui-dialog-titlebar-close").hide();
        },
        buttons: {
            Ok: function () {
                $(this).dialog("close");
                $(this).remove();
                location.reload();
            }
        }
    });
}

function exibirMensagemErro(titulo, mensagem) {
    $.bigBox({
        title: titulo,
        content: mensagem,
        color: "#C46A69",
        timeout: 6000,
        icon: "fa fa-warning shake animated",
        //number: "1",
        timeout: 6000
    });
}

