//
//  ContentView.swift
//  Pantry
//
//  Created by Matthias Röhricht on 29.12.24.
//

import SwiftUI
import SwiftData

struct ContentView: View {
    var body: some View {
        TabView {
            GoodsView()
                .tabItem {
                    Label("Goods", systemImage: "basket")
                }
            BlankPage()
                .tabItem {
                    Label("Plan", systemImage: "calendar")
                }
            RecipeView()
                .tabItem {
                    Label("Recipes", systemImage: "book.pages")
                }
            MenuView()
                .tabItem {
                    Label("Menu", systemImage: "list.dash")
                }
        }
    }
}

#Preview {
    ContentView()
}
