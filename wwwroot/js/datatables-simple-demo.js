window.addEventListener('DOMContentLoaded', event => {
    // Simple-DataTables
    // https://github.com/fiduswriter/Simple-DataTables/wiki

    const datatablesSimple = document.getElementById('datatablesSimple');
    if (datatablesSimple) {
        new simpleDatatables.DataTable(datatablesSimple);
    }

    const datatablesSimpleTickets = document.getElementById('datatablesSimpleTickets');
    if (datatablesSimpleTickets) {
        new simpleDatatables.DataTable(datatablesSimpleTickets);
    }

    const datatablesSimpleAllProjects = document.getElementById('datatablesSimpleAllProjects');
    if (datatablesSimpleAllProjects) {
        new simpleDatatables.DataTable(datatablesSimpleAllProjects);
    }
});
