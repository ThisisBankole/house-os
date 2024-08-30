module.exports = function override(config, env) {
    config.devServer = {
        ...config.devServer,
        setupMiddlewares: function (middlewares, devServer) {
            devServer.app.use((req, res, next) => {
                console.log('Custom middleware');
                next();
            });

            return middlewares;
        },
    };
    return config;
};