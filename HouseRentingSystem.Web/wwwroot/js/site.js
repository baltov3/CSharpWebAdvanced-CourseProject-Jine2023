function statistics() {
   
    $('#statistics_btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();


        $.get('https://localhost:7006/api/statistics',  (data) => {
            $('#total_houses').text(data.totalHouses + " Houses");
            $('#total_rents').text(data.totalRents + " Rents");

            $('#statistic_box').removeClass('d-done');
            $('#statistics_btn').hide();

        });
    });
    
}
