$(function () {

    var count = 0;
    var current = 0;

    $("#btnGo").on("click", function () {

        current = 0;
        var cedula = $("#txtCedula").val();
                        
        $(this).text("Procesando...").prop("disabled", true);
        $("#result").text("");
        
        $.get("/home/getRifList", { cedula: cedula })
          .done(function (data) {
              
              count = data.length;
                            
              data.forEach(function (e, i) {
                  $.get("/home/GetCompanyName", { rif: e })
                    .done(function (data)
                    {                        
                        current++;
                       
                        $("#result").append(data.Name + " Rif:" + data.Rif + "<br />");

                        if(current==count)
                            $("#btnGo").text("Consultar").prop("disabled", false);
                    });
              });

              if(count == 0)
              {
                  $("#result").text("No hay empresas relacionadas");
                  $("#btnGo").text("Consultar").prop("disabled", false);
              }
                  


          });
        
        return false;
    });
});